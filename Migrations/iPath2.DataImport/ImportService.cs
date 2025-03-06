using iPath.Data.Configuration;
using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using iPath.Data;
using iPath.Application.Services.Storage;
using Hl7.Fhir.Utility;
using iPath.Data.EFCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;


namespace iPath2.DataImport;


public class MessageEventArgs : EventArgs
{
    public string Message { get; }
    public MessageEventArgs(string Msg)
    {
        Message = Msg;
    }
}
public class ProgressEventArgs : EventArgs
{
    public int Progress { get; }
    public string Message { get; }
    public ProgressEventArgs(int val, string msg)
    {
        Progress = val;
        Message = msg;
    }

}


public interface IImportService
{
    Task<bool> ImportUsersAsync(bool dropExisintg, CancellationToken ctk = default);
    Task<bool> ImportCommunitiesAsync(bool dropExisintg, CancellationToken ctk = default);
    Task<bool> ImportGroupsAsync(bool dropExisintg, CancellationToken ctk = default);
    Task<bool> ImportNodesAsync(bool deleteExitingData, bool reImportAll, HashSet<int> groupIds = null!, CancellationToken ctk = default);
    string JsonDataPath { get; }
    int BulkSize { get; set; }


    delegate void MessageHandler(object sender, MessageEventArgs e);
    event MessageHandler MessageEvent;

    delegate void ProgressHandler(object sender, ProgressEventArgs e);
    event ProgressHandler ProgressEvent;
}


public class ImportService(IOptions<iPathConfig> opts, 
    IDbContextFactory<OldDB> oldFct, 
    IDbContextFactory<NewDB> newFct, 
    IStorageService srvStorage) 
    : IImportService
{
    private iPathConfig cfg => opts.Value;
    public string JsonDataPath => srvStorage.StoragePath; 
    public int BulkSize { get; set; } = 1000;


    #region "-- Events --"
    public event IImportService.MessageHandler MessageEvent;
    public event IImportService.ProgressHandler ProgressEvent;

    protected virtual void OnMessage(string msg)
    {
        MessageEvent?.Invoke(this, new MessageEventArgs(msg));
    }

    protected virtual void OnProgress(int val, string msg)
    {
        ProgressEvent?.Invoke(this, new ProgressEventArgs(val, msg));
    }
    #endregion


    public async Task<bool> ImportUsersAsync(bool dropExisintg, CancellationToken ctk = default)
    {
        using var oldDb = await oldFct.CreateDbContextAsync();
        using var newDb = await newFct.CreateDbContextAsync();

        if( dropExisintg )
        {
            var total = await newDb.Users.CountAsync(ctk);
            OnMessage($"daleting {total} existing users");
            await newDb.Users.ExecuteDeleteAsync(ctk);
        }

        // check new roles
        UserRole? arole = null!;
        if (newDb.UserRoles.Count() == 0)
        {
            var x = newDb.UserRoles.Add(UserRole.Admin);
            newDb.UserRoles.Add(UserRole.Moderator);
            await newDb.SaveChangesAsync();
            arole = x.Entity;
        }


        var userCount = oldDb.persons.Count();
        OnMessage($"reading {userCount} Users ... ");
        var users = oldDb.persons.AsNoTracking().AsAsyncEnumerable();

        var bulk = new List<User>();
        var c = 0;
        await foreach (var u in users)
        {
            if (ctk.IsCancellationRequested)
            {
                ctk.ThrowIfCancellationRequested();
            }

            // insert into new DB
            var newUser = u.ToNewEntity();

            if (newUser.Id == 1 && arole != null ) newUser.Roles.Add(arole);

            bulk.Add(newUser);
            c++;
            OnProgress((int)((float)c * 100 / userCount), $"{c}/{userCount}");

            if (c % BulkSize == 0)
            {
                await BulkInsertAsync(newDb, bulk, ctk);
                bulk.Clear();
            }
        }
        
        if( bulk.Any() ) await BulkInsertAsync(newDb, bulk, ctk);
        bulk.Clear();

        var newCount = newDb.Users.Count();
        OnMessage($"{newCount} Users imported");

        var reseedMsg = await ReseedTable<User>(newDb);
        OnMessage($"Table reseed: {reseedMsg}");

        return true;
    }


    public async Task<bool> ImportCommunitiesAsync(bool dropExisintg, CancellationToken ctk = default)
    {
        using var oldDb = oldFct.CreateDbContext();
        using var newDb = newFct.CreateDbContext();

        if (dropExisintg)
        {
            var total = newDb.Communities.Count();
            OnMessage($"daleting {total} existing communities");
            await newDb.Communities.ExecuteDeleteAsync();
        }

        var list = await oldDb.Set<i2community>().ToListAsync();
        OnMessage($"exporting {list.Count} communities ... ");
        var newList = list.Select(o => o.ToNewEntity()).ToList();

        OnMessage("saving changes ...");
        await BulkInsertAsync(newDb, newList, ctk);
        OnProgress(100, $"{list.Count}/{newList.Count}");

        OnMessage($"{newList.Count()} communities imported");

        var reseedMsg = await ReseedTable<Community>(newDb);
        OnMessage($"Table reseed: {reseedMsg}");

        return true;
    }


    public async Task<bool> ImportGroupsAsync(bool dropExisintg, CancellationToken ctk = default)
    {
        using var oldDb = oldFct.CreateDbContext();
        using var newDb = newFct.CreateDbContext();

        if (dropExisintg)
        {
            var x = newDb.Groups.Count();
            OnMessage($"daleting {x} existing groups");
            await newDb.Set<GroupMember>().ExecuteDeleteAsync();
            await newDb.Groups.ExecuteDeleteAsync();
        }

        // load the Community Cache
        DataImportExtensions.communityCache = new();
        foreach( var comm in await newDb.Communities.AsNoTracking().ToListAsync())
        {
            DataImportExtensions.communityCache.Add(comm.Id, comm);
        }

        await PrepareOwnerCache(newDb);


        var q = oldDb.Set<i2group>()
            .Where(g => g.id > 1)                 // skip old admnin group with id = 1
            .Include(g => g.members)
            .Include(g => g.communities)
            .AsNoTracking();

        var total = await q.CountAsync();

        var c = 0;
        var bulk = new List<Group>();
        OnMessage($"exporting {total} groups (bulk size = {BulkSize}) ... ");
        await foreach (var group in q.AsAsyncEnumerable())
        {
            if( ctk.IsCancellationRequested )
            {
                ctk.ThrowIfCancellationRequested();
            }

            // entity
            var n = group.ToNewEntity();

            // members
            n.Members ??= new List<GroupMember>();
            foreach( var gm in group.members )
            {
                // validate that the user_id is valid (contained in ownerCache)
                if (DataImportExtensions.usernameCache.ContainsKey(gm.user_id) && !n.Members.Any(m => m.UserId == gm.user_id))
                {
                    // old status => role: 0=member, 4=moderator, 2=inactive, 8=guest

                    eMemberRole role = eMemberRole.User;
                    if ((gm.status & 4) != 0) role = role | eMemberRole.Moderator;
                    if ((gm.status & 2) != 0) role = role | eMemberRole.Inactive;
                    if ((gm.status & 8) != 0) role = role | eMemberRole.Guest;

                    var newGM = new GroupMember()
                    {
                        GroupId = gm.group_id,
                        UserId = gm.user_id,
                        Role = role
                    };
                    n.Members.Add(newGM);
                }
                else
                {
                    Console.WriteLine("invalid member: " + gm.ToString());
                }
            }

            // count up
            c++;
            OnProgress((int)((float)c * 100 / total), $"{c}/{total}");

            bulk.Add(n);

            if (c % BulkSize == 0)
            {
                await BulkInsertAsync(newDb, bulk, ctk);
                bulk.Clear();
            }
        }

        OnMessage($"{c} groups converted ... saving to database ... ");
        if(bulk.Any()) await BulkInsertAsync(newDb, bulk, ctk);
        bulk.Clear();

        var reseedMsg = await ReseedTable<Group>(newDb);
        OnMessage($"Table reseed: {reseedMsg}");

        return true;
    }



    public async Task<bool> ImportNodesAsync(bool deleteExitingData, bool reImportAll, HashSet<int> groupIds = null!, CancellationToken ctk = default)
    {
        using var oldDb = oldFct.CreateDbContext();
        using var newDb = newFct.CreateDbContext();

        OnMessage("Data Path: " + cfg.TempDataPath);
        if (!Directory.Exists(cfg.TempDataPath))
        {
            OnMessage("not found");
            return false;
        }

        // check for topparent links
        OnMessage("checking pre-migration ... ");
        var tc = await oldDb.Set<i2object>().CountAsync(o => o.topparent_id != null);
        if (tc == 0)
        {
            OnMessage("please execute the 'prepare-migration.sql' first");
            return false;
        }

        await PrepareOwnerCache(newDb);
        await PrepareGroupCache(newDb);

        // get group list => all groups if nothin specified (ignore fix admin group with id = 1)
        if( groupIds is null )
        {
            groupIds = await oldDb.Set<i2group>().Where(g => g.id > 1).Select(g => g.id).ToHashSetAsync();
        }

        if( groupIds.Count < 5)
        {
            OnMessage("Exporting groups: " + string.Join(", ", groupIds));
        }
        else
        {
            OnMessage($"{groupIds.Count} groups to be exported");
        }

        await ImportGroupNodesAsync(deleteExitingData, reImportAll, groupIds, newDb, oldDb, ctk);


        return true;
    }

    public async Task<bool> ImportGroupNodesAsync(bool deleteExitingData, bool reImportAll, HashSet<int> gid, NewDB newDb, OldDB oldDb, CancellationToken ctk = default)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        if (deleteExitingData)
        {
            // Delete Nodes in New DB
            var ec = await newDb.Nodes.CountAsync(n => n.GroupId.HasValue && EF.Constant(gid).Contains(n.GroupId.Value));

            OnMessage($"deleting existing {ec} nodes ... ");
            await newDb.Nodes.Where(n => n.GroupId.HasValue && EF.Constant(gid).Contains(n.GroupId.Value)).ExecuteDeleteAsync(ctk);
            OnMessage("done");
        }


        var q = oldDb.Set<i2object>()
            .Where(o => o.objclass != "imic")
            .Where(o => o.group_id.HasValue && EF.Constant(gid).Contains(o.group_id.Value))
            .Where(o => !o.parent_id.HasValue)
            .Where(o => o.sender_id > 0)
            .Include(o => o.ChildNodes)
            .Include(o => o.Annotations)
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        if( !reImportAll)
        {
            q = q.Where(o => !o.ExportTime.HasValue);
        }

        var total = await q.CountAsync(ctk);
        OnMessage($"Starting import of {total} root objects ... ");


        var objects = q.OrderBy(o => o.id).AsAsyncEnumerable();

        // debug => BulkSize = 1;
        var nodeBulk = new List<Node>();
        var annotationBulk = new List<Annotation>();
        var c = 0;

        await foreach (var o in objects)
        {
            c++;

            if( ctk.IsCancellationRequested)
            {
                ctk.ThrowIfCancellationRequested(); 
            }

            // nodes (incl. ChildNodes)
            var n = o.ToNewEntity();
            nodeBulk.Add(n);

            // child nodes => sender mus be > 0 and there there must be something in the old data field
            if (o.ChildNodes != null && o.ChildNodes.Any())
            {
                nodeBulk.AddRange(o.ChildNodes.Where(c => c.sender_id > 0 && !string.IsNullOrEmpty(c.data)).Select(o => o.ToNewEntity()));
            }

            // annotations
            annotationBulk.AddRange(o.Annotations.Where(a => a.sender_id > 0).Select(a => a.ToNewEntity()));
                     
            OnProgress((int)(double)(c * 100 / total), $"{c} / {total}");

            if (c % BulkSize == 0)
            {
                // node Bulk
                OnMessage($"saving {nodeBulk.Count()} nodes (incl child nodes) ... ");
                await SaveNodeImportAsync(nodeBulk, newDb, oldDb, ctk);

                // annotations
                await BulkInsertAsync(newDb, annotationBulk, ctk);
                annotationBulk.Clear();
            }
        }

        OnMessage("saving remaining changes ... ");
        await SaveNodeImportAsync(nodeBulk, newDb, oldDb, ctk);
        nodeBulk.Clear();

        await BulkInsertAsync(newDb, annotationBulk, ctk);
        annotationBulk.Clear();

        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        OnMessage($"{c} nodes exported in {ts.TotalSeconds}s");



        var reseedNodeMsg = await ReseedTable<Node>(newDb);
        OnMessage($"Table reseed: {reseedNodeMsg}");

        var reseedAnnotationMsg = await ReseedTable<Annotation>(newDb);
        OnMessage($"Table reseed: {reseedAnnotationMsg}");

        return true;
    }


    private async Task SaveNodeImportAsync(List<Node> bulk, NewDB newDb, OldDB oldDb, CancellationToken ctk = default)
    {
        if (bulk != null && bulk.Any())
        {
            // create list of ids to update in old db
            var objIds = bulk.Select(n => n.Id).ToHashSet();

            // delete alread imported NodeDatan (data/info fields with old xml)
            await newDb.Set<NodeImport>().Where(d => EF.Constant(objIds).Contains(d.NodeId)).ExecuteDeleteAsync();

            // bulk insert in new db
            await BulkInsertAsync(newDb, bulk, ctk);


            // export root nodes to json
            if(opts.Value.ExportNodeJson)
            {
                OnMessage("Export Nodes to Json files");
                foreach ( var node in bulk.Where(n => n.GroupId.HasValue) )
                {
                    await srvStorage.PutNodeJsonAsync(node);
                }
            }

            bulk.Clear();

            // update export flag
            await oldDb.Set<i2object>()
                .Where(o => EF.Constant(objIds).Contains(o.id))
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ExportTime, DateTime.UtcNow), ctk);

        }
    }



    private async Task BulkInsertAsync<T>(DbContext ctx, List<T> bulk, CancellationToken ctk = default) where T : BaseEntity
    {
        var sw = new Stopwatch();
        sw.Start();
        using (var transaction = ctx.Database.BeginTransaction())
        {
            var entityType = ctx.Model.FindEntityType(typeof(T));
            var schema = entityType.GetSchema();
            string? tableName = entityType.GetTableName();

            // check for update or insert
            var bulkIds = bulk.Select(x => x.Id).ToHashSet();
            var dbIds = await ctx.Set<T>().Where(e => EF.Constant(bulkIds).Contains(e.Id)).Select(e => e.Id).ToListAsync();

            var insertBulk = bulk.Where(b => !dbIds.Contains(b.Id)).ToList(); // list for insert
            var updateBulk = bulk.Where(b => dbIds.Contains(b.Id)).ToList();  // list for update

            OnMessage($"Saving {tableName}, Insert: {insertBulk.Count}, Update: {updateBulk.Count}");

            if(insertBulk != null && insertBulk.Any())
            {
                await ctx.Set<T>().AddRangeAsync(insertBulk);
            }
            if( updateBulk != null && updateBulk.Any())
            {
                await ctx.AddRangeAsync(updateBulk);
                ctx.UpdateRange(updateBulk);
            }



            if (cfg.DbProvider == DBProvider.SqlServer.Name && tableName != null)
            {
                // for SqlServer activiate Identity Insert
                await ctx.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableName} ON;");
                await ctx.SaveChangesAsync(ctk);
                await ctx.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableName} OFF;");
            }
            else
            {
                // for other ds just save
                await ctx.SaveChangesAsync(ctk);
            }

            transaction.Commit();
        }

        // release entities from change tracker               
        ctx.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Detached)
            .ToList()
            .ForEach(e => e.State = EntityState.Detached);

        ctx.ChangeTracker.Clear();

        sw.Stop();
        OnMessage($"transaction time: {sw.ElapsedMilliseconds}ms");
    }



    private async Task<string> ReseedTable<T>(NewDB ctx) where T : BaseEntity
    {
        var entityType = ctx.Model.FindEntityType(typeof(T));
        var schema = entityType.GetSchema();
        string? tableName = entityType.GetTableName();

        if (string.IsNullOrEmpty(tableName)) return $"table for {entityType.Name} no found";

        int maxId;
        if (cfg.DbProvider == DBProvider.Postgres.Name )
        {
            maxId = await ctx.Set<T>().MaxAsync(e => e.Id);
            await ctx.Database.ExecuteSqlRawAsync($"ALTER TABLE public.\"Users\" ALTER \"Id\" RESTART {maxId + 1}");
            return $"{tableName} reseeded to {maxId + 1}";
        }

        return $"no reseed for {cfg.DbProvider}";
    }




    private async Task PrepareOwnerCache(NewDB newDb)
    {
        if(DataImportExtensions.usernameCache is null )
        {
            Console.Write("loading owner cache ... ");
            DataImportExtensions.usernameCache = new();
            var users = await newDb.Users.AsNoTracking().Select(u => new Tuple<int, string>(u.Id, u.Username)).ToListAsync();
            users.ForEach(u => DataImportExtensions.usernameCache.Add(u.Item1, u.Item2));
            Console.WriteLine("{0} users", DataImportExtensions.usernameCache.Count);
        }
    }

    private async Task PrepareGroupCache(NewDB newDb)
    {
        if (DataImportExtensions.groupNameCache is null)
        {
            Console.Write("loading group cache ... ");
            DataImportExtensions.groupNameCache = new();
            var groups = newDb.Groups.AsNoTracking().Select(g => new { g.Id, g.Name }).ToList();
            groups.ForEach(g => DataImportExtensions.groupNameCache.Add(g.Id, g.Name));
            Console.WriteLine("{0} groups", DataImportExtensions.groupNameCache.Count);
        }
    }
}
