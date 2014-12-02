$(function () {

    var hubs = document.createElement("script");
    hubs.src = "http://localhost:9000/signalr/hubs";
    hubs.type = "text/javascript";
    $("document, head").append(hubs);

    setTimeout(function() {

        // Set the hubs URL for the connection
        $.connection.hub.url = "/signalr";

        // Declare a proxy to reference the hub.
        var widgt = $.connection.widgtSignalRHub;

        // Create a function that the hub can call to broadcast messages.
        widgt.client.widgetDeployed = function (widgetId) {
            // reload the page as the widget has been redeployed (could actually be any widget, not just this one)
            window.location.href = window.location.href;
        };

        // Start the connection.
        $.connection.hub.start({ jsonp: true }).done(function () {
            console.log("Hub connected");
        });
    }, 1000);
});