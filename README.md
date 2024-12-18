### ASP .NET web services

| **Name**                  | **Ports**         | **Description**                                                                                                                                                       |
|---------------------------|-------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Northwind.Common**      | N/A               | A class library project for common types like interfaces, enums, classes, records, and structs, used across multiple projects.                                       |
| **Northwind.EntityModels**| N/A               | A class library project for common EF Core entity models. Entity models are often used on both the server and client side, so it is best to separate dependencies.   |
| **Northwind.DataContext** | N/A               | A class library project for the EF Core database context with dependencies on specific database providers.                                                           |
| **Northwind.UnitTests**   | N/A               | An xUnit test project for the solution.                                                                                                                               |
| **Northwind.Web**         | http 5130, https 5131 | An ASP.NET Core project for a simple website that uses a mixture of static HTML files and dynamic Razor Pages.                                                        |
| **Northwind.Mvc**         | http 5140, https 5141 | An ASP.NET Core project for a complex website that uses the MVC pattern and can be more easily unit tested.                                                           |
| **Northwind.WebApi**      | http 5150, https 5151 | An ASP.NET Core project for a Web API, aka HTTP service. A good choice for integrating with websites because it can use JavaScript libraries or Blazor to interact.  |
| **Northwind.MinimalApi**  | http 5152         | An ASP.NET Core project for a Minimal API, which can be compiled using native AOT for improved startup time and reduced memory footprint.                            |
| **Northwind.Blazor**      | http 5160, https 5161 | An ASP.NET Core Blazor project.                                                                                                                                      |

This table lists example project names, their associated port numbers for local hosting, and a description of their purpose in a structured solution. It follows a systematic convention for managing multiple projects in a solution.