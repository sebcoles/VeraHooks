import React, { Component } from 'react';
import AsyncSelect from 'react-select/async';
import "react-datepicker/dist/react-datepicker.css";

class Webhook extends Component {
    constructor(props) {
        super(props);
        this.state = {
            name: null,
            secondsBetweenCheck: null,
            sendAddress: null
        };

        this.setName = this.setName.bind(this);
        this.setDemand = this.setDemand.bind(this);
        this.setSecondsBetweenCheck = this.setSecondsBetweenCheck.bind(this);
        this.setSendAddress = this.setSendAddress.bind(this);
        this.get = this.get.bind(this);
        this.getAgents = this.getAgents.bind(this);
    }

    setName(event) { this.props.setName(event.target.value); }
    setDemand(event) { this.props.setDemand(event.value); }
    setSecondsBetweenCheck(event) { this.props.setSecondsBetweenCheck(parseInt(event.target.value)); }
    setSendAddress(event) { this.props.setSendAddress(event.target.value); }

    getAgents() {
        return this.get('Agent/Get');
    }

    get(url) {
        return fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        }).then((res) => {
            return res.json();
        }).then((x) => {
            return x.map(y => ({ label: y.name, value: y.name }) );
        });
    }

    render() {
        return (
            <div>
                <h2>Webhook Configuration</h2>
                    <div className="form-group">
                        <label htmlFor="name">Webook Name:</label>
                        <input
                            className="form-control"
                            type="text"
                            value={this.props.name}
                            onChange={this.setName}
                    />
                    <label htmlFor="name">Agent Name:</label>
                    <AsyncSelect
                        cacheOptions
                        defaultOptions
                        onChange={this.setDemand}
                        loadOptions={this.getAgents}
                    />          
                    <label htmlFor="secondsBetweenCheck">Seconds between checks?</label>
                        <input
                            className="form-control"
                            type="number"
                            value={this.props.secondsBetweenCheck}
                            onChange={this.setSecondsBetweenCheck}
                        />
                        <label htmlFor="sendAddress">What web address should webhook POST to?</label>
                        <input
                            className="form-control"
                            type="text"
                            value={this.props.sendAddress}
                            onChange={this.setSendAddress}
                        />
                    </div>                   
            </div>
        );
    }
}
export default Webhook;

