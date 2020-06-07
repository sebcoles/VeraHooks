import React, { Component } from 'react';
import { toast } from 'react-toastify';


export class List extends Component {
    static displayName = List.name;

  constructor(props) {
    super(props);
      this.state = { webhooks: [], selected: null };

      this.deleteWebhook = this.deleteWebhook.bind(this)
      this.getWebhooks = this.getWebhooks.bind(this);
      this.get = this.get.bind(this);
      this.webhookSelected = this.webhookSelected.bind(this);

      this.getWebhooks();
      this.interval = setInterval(() => this.getWebhooks(), 10000);
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }


    getWebhooks() {
        return this.get('Webhook').then(function (data) {
            this.setState({ webhooks: data });
        }.bind(this));
    }

    webhookSelected(webhook) {
        this.setState({ selected: webhook });
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

    deleteWebhook(event) {
        return fetch(`Webhook/Delete?id=${event.target.id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then(function(response) {
            if (response.ok) {
                toast.success('Webhook removed OK!')
            } else {
                toast.error('Something went wrong!')
            }
        }).then(this.getWebhooks.bind(this));
    }
  

  render() {
    return (
      <div>
            <h2 id="tabelLabel" >Webhooks</h2>
            <div className="form-group">
                <label htmlFor="name" style={{fontWeight: 'bold'}}>Send Address: </label>
                <label htmlFor="name">{this.state.selected != null ? this.state.selected.sendAddress : ''}</label>
            </div>
            <div className="form-group">
                <label htmlFor="name" style={{ fontWeight: 'bold' }}>Delay: </label>
                <label htmlFor="name">{this.state.selected != null ? this.state.selected.secondsBetweenCheck : ''}</label>
            </div>
            <div className="form-group">
                <label htmlFor="name" style={{ fontWeight: 'bold' }}>Last Fired: </label>
                <label htmlFor="name">{this.state.selected != null ? this.state.selected.lastFired : ''}</label>
            </div>
            <div className="form-group">
                <label htmlFor="name" style={{ fontWeight: 'bold' }}>Created: </label>
                <label htmlFor="name">{this.state.selected != null ? this.state.selected.created : ''}</label>
            </div>
            <table style={{ height: 150, overflowy:'scroll'}} className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Apps</th>
                        <th>Conditions</th>
                        <th>Times Fired</th>
                        <th>Delete</th>
                    </tr>
                </thead>
                <tbody>
                {this.state.webhooks.map(webhook =>
                    <tr style={{ background: this.state.selected != null && webhook.id == this.state.selected.id ? 'whitesmoke' : 'white'}}
                        onClick={() => this.webhookSelected(webhook)}
                        key={webhook.id}>
                            <td>{webhook.name}</td>
                            <td>{webhook.apps.map(x => x.appName).join(',')}</td>
                            <td>{webhook.propertyConditions.map(x => `${x.field} is equal to ${x.expectedValue}`).join(',')}</td>
                            <td>{webhook.timesFired}</td>
                            <td><button
                                id={webhook.id}
                                type="button"
                                onClick={this.deleteWebhook.bind(this)}
                            >-</button></td>
                        </tr>
                    )}
                </tbody>
            </table>
      </div>
    );
  }
}
