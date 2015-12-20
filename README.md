Widgt
=====

A .NET deployment environment for [W3C Widgets](http://www.w3.org/TR/widgets/) with the folloing features:

## Configuration file localization
Supports localized text inside of the deployment manifest files

## Folder based localization of resources
Support for locale sensitive web assets within the package (plced under separate locale folders)

## Support for features
Client side features can be registered on the server and injected into a widget.  Enables reuse of library code without the need for redistributing in every package.

Widgt also supports server side features written in IronPython. This enables a deployed widget to contain some level of server side logic as well as client side.

## SignalR based notification system
Deployment notifications can be pushed to the container environment (web page) to enable live reloading of widget clients.

## REST API provided using WebApi
Deployment of widgets is done via a REST interface using WebApi

## OWIN middleware
THe server has been written using OWIN middleware.  This means it can be hosted in a variety of places (IIS, self hosted, Linux, Mac)

## Database persistance using code first Entity Framework 6
The persistence layer is pluggable, but by default uses Code First Entity Framework.  An example using SQL Server CE is provided.




