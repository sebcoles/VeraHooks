import React, { Component } from 'react';
import { toast } from 'react-toastify';

export class Agents extends Component {
    static displayName = Agents.name;

  constructor(props) {
    super(props);
      this.state = { agents: [], loading: true };

      this.deleteAgent = this.deleteAgent.bind(this)
      this.getAgents = this.getAgents.bind(this);
      this.get = this.get.bind(this);

      this.interval = setInterval(() => this.getAgents(), 1000);
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }

    getAgents() {
        return this.get('Agent/Get').then(function (data) {
            this.setState({ agents: data });
        }.bind(this));
    }

    get(url) {
        return fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        }).then((res) => {
            return res.json();
        });
    }

    deleteAgent(event) {
        return fetch(`Agent/Delete?name=${event.target.id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then(function(response) {
            if (response.ok) {
                toast.success('Agent removed OK!')
            } else {
                toast.error('Something went wrong!')
            }
        }).then(this.getAgents.bind(this));
    }
  

  render() {
    return (
      <div>
        <h1 id="tabelLabel" >Agents</h1>
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Ip Address</th>
                        <th>Agent</th>
                        <th>Veracode</th>
                        <th>Database</th>
                        <th>Delete</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.agents.map(Agent =>
                        <tr key={Agent.name}>
                            <td>{Agent.name}</td>
                            <td>{Agent.ipAddress}</td>
                            <td>{Agent.last30Seconds ? 'OK' : 'Down'}</td>
                            <td>{(Agent.last30Seconds && Agent.veracodeOk) ? 'OK' : 'Down'}</td>
                            <td>{(Agent.last30Seconds && Agent.databaseOk) ? 'OK' : 'Down'}</td>
                            <td><button
                                id={Agent.name}
                                type="button"
                                onClick={this.deleteAgent.bind(this)}
                            >-</button></td>
                        </tr>
                    )}
                </tbody>
            </table>
      </div>
    );
  }
}
