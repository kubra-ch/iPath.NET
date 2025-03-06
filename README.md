# iPath.NET

This project is intended to replace the functionality of the iPath Telemedicine platform (https://sourceforge.net/projects/ipath/). 
While the original software had been written in PHP, this new version is written in c# and using the .net core platform. 
The UI is written in Blazor, using the MudBlazor component library.

At present the system is running in Blazor server mode and does not yet expose an API. 
However the dataflow is following a request response pattern over mediator and it is intended 
to factor out a complete API later. Eventually the UI will be tranformed to a web assembly 
running on the client browser.


## Project Structure
The project is structured loosely following a Vertical Slice Architecture pattern.

- **iPath.Data:** Classlibrary containing the entities and the entity framework configuration
- **iPath.Application:** The application logic is following a CQRS idea using mediator to route queries and commands. Domain Events are publishing over mediator
- **Migrations:** Migrations assemblies for various database systems.
- **iPath.Data.EFCore:** a helper assembly for configurung and injecting the EFCore DB Context
- **iPath.API:** API layer. At present used only for accessing images and hosting a SignalR hub for realtime client updates.
- **iPath.UI:** The main UI written in Blazor.

