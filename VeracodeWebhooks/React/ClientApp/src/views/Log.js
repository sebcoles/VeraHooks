import React, { Component } from 'react';

export class Log extends Component {
    static displayName = Log.name;

    constructor(props) {
        super(props);
        this.state = { logs: [], loading: true };
    }

    componentDidMount() {
        this.populateLogsData();
    }

    static renderLogs(logs) {
        return (
            <p>
                {logs.map(log =>
                    <div style={{ fontSize: 12 }}>{`[${log.timeFired}] : Fired '${log.webHookName}' which sent a POST request to ${log.webHookSendAddress} and recieved HTTP Status ${log.statusReturned}`}<br /></div>
                )}
            </p>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Log.renderLogs(this.state.logs);

        return (
            <div>
                <h1 id="tabelLabel" >Logs</h1>
                {contents}
            </div>
        );
    }

    async populateLogsData() {
        const response = await fetch('Webhook/Logs');
        const data = await response.json();
        this.setState({ logs: data, loading: false });
    }
}
