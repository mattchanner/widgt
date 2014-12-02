
namespace Embedding.Client
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR.Client;

    /// <summary>
    /// A hub client for deployment notifications
    /// </summary>
    public class WidgtHubClient
    {
        /// <summary> The hub connection </summary>
        private readonly HubConnection connection;

        /// <summary> The subject to be used when pushing deployment notifications </summary>
        private readonly Subject<string> deployerSubject = new Subject<string>();

        /// <summary> The subject to be used when pushing un-deployment notifications </summary>
        private readonly Subject<string> undeploySubject = new Subject<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgtHubClient"/> class.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public WidgtHubClient(string endpoint = "http://localhost:9000")
        {
            connection = new HubConnection(endpoint);
            IHubProxy proxy = connection.CreateHubProxy("WidgtSignalRHub");
            proxy.On<string>("widgetDeployed", widgetId => this.deployerSubject.OnNext(widgetId));
            proxy.On<string>("widgetUndeployed", widgetId => this.undeploySubject.OnNext(widgetId));
        }

        /// <summary>
        /// Starts the client
        /// </summary>
        /// <returns>The task to wait on</returns>
        public async Task Start()
        {
            await connection.Start();
        }

        /// <summary>
        /// Sends notifications when a widget is deployed
        /// </summary>
        public IObservable<string> WidgetDeployed
        {
            get { return deployerSubject.AsObservable(); }
        }

        /// <summary>
        /// Sends notifications when a widget is un-deployed
        /// </summary>
        public IObservable<string> WidgetUndeployed
        {
            get { return undeploySubject.AsObservable(); }
        }
    }
}
