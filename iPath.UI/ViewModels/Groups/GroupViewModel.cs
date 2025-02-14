﻿using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Groups;

public class GroupViewModel(IDataAccess srvData) : IGroupViewModel
{
    private GroupModel _model;
    public GroupModel Model => _model;

    private GridItemsProvider<NodeModel> _NodesDataProvider = default!;
    public GridItemsProvider<NodeModel> NodesDataProvider => _NodesDataProvider;

    private string _error = default!;
    public string ErrorMessage => _error;

    public async Task LoadGroupAsync(int Id)
    {
        var rg = new GetGroupQuery(GroupId: Id);
        var respg = await srvData.Send(rg);
        if ( !respg.Success )
        {
            _error = respg.Message;
        }

        _model = new GroupModel()
        {
            Id = respg.Data.Id,
            Name = respg.Data.Name,
            Owner = respg.Data.Owner,
            Purpose = respg.Data.Purpose
        };


        // get nodes
        var request = new GetNodeListQuery()
        {
            GroupId = Id,
        };

        // get datae
        _NodesDataProvider = async req =>
        {
            request.StartIndex = req.StartIndex;
            request.Count = req.Count;

            // sorting
            request.SortDefinitions = new();
            var sort = req.GetSortByProperties();
            if (sort != null && sort.Any())
            {
                foreach (var p in sort)
                {
                    if (p.PropertyName == "OwnerName")
                    {
                        var sd = new SortDefinition { SortColumn = "Owner.Username", SortAscending = (p.Direction == SortDirection.Ascending) };
                        request.SortDefinitions.Add(sd);
                    }
                    else
                    {
                        var sd = new SortDefinition { SortColumn = p.PropertyName, SortAscending = (p.Direction == SortDirection.Ascending) };
                        request.SortDefinitions.Add(sd);
                    }
                }
            }
            request.SortDefinitions.Add(new SortDefinition { SortColumn = "CreatedOn", SortAscending = false });

            var response = await srvData.Send(request);
            if (!response.Success)
            {
                _error = respg.Message;
                throw new Exception(respg.Message);
            }

            var models = new List<NodeModel>();
            foreach (var node in response.Data.Items)
            {
                models.Add(new NodeModel(node, respg.Data));
            }


            return GridItemsProviderResult.From(
                items: models,
                totalItemCount: response.Data.TotalItemsCount
                );
        };
    }

    public async Task<NodeCommandRespone> CreateNodeAsync(int UserId, string Title, NodeType nodeType)
    {
        var req = new CreateNodeCommand()
        {
            OwnerId = UserId,
            GroupId = Model.Id,
            NodeType = nodeType,
            Title = Title 
        };
        return await srvData.Send(req);
    }
}
