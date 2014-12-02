Widgt
=====

Widgt provides a deployment environment for [W3C Widgets](http://www.w3.org/TR/widgets/).

The server is implemented as a set of OWIN middleware components, using WebApi to provide a simple REST interface for deploying new widgets by POSTing widget files to the /api/widgt endpoint address.

SignalR can also be used for receiving deployment notifications.

Feature support is implemented, along with server side features written in IronPython.

A self hosted web server is provided as an example, showing how the Widgt framework is configured (see StartUp.cs)
