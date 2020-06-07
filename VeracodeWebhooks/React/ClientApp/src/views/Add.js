import React, { Component } from 'react';
import Webhook from '../components/Webhook'
import Entity from '../components/Entity'
import Condition from '../components/Condition'
import { toast } from 'react-toastify';
import "react-datepicker/dist/react-datepicker.css";

export class Add extends Component {
    static displayName = Add.name;

    constructor(props) {
        super(props);
        this.state = {
            name: null,
            secondsBetweenCheck: null,
            sendAddress: null,
            apps: null,
            demand: null,
            conditions: []
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.setName = this.setName.bind(this);
        this.setDemand = this.setDemand.bind(this);
        this.setSecondsBetweenCheck = this.setSecondsBetweenCheck.bind(this);
        this.setSendAddress = this.setSendAddress.bind(this);
        this.setApps = this.setApps.bind(this);
        this.addCondition = this.addCondition.bind(this);
        this.removeCondition = this.removeCondition.bind(this);
    }

    handleSubmit() {
        fetch('Webhook/Add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                webhook: {
                    Name: this.state.name,
                    SecondsBetweenCheck: this.state.secondsBetweenCheck,
                    SendAddress: this.state.sendAddress,
                    Demand: this.state.demand
                },
                apps: this.state.apps,
                conditions: this.state.conditions
            })
        }).then(function (response) {
            if (response.ok) {
                toast.success('Webhook saved OK!')
            } else {
                toast.error('Something went wrong!')
            }
        })
    }

    setName(value) { this.setState({ name: value }); }
    setDemand(value) { this.setState({ demand: value }); }
    setSecondsBetweenCheck(value) { this.setState({ secondsBetweenCheck: value }); }
    setSendAddress(value) { this.setState({ sendAddress: value }); }
    setApps(value) { this.setState({ apps: value }); }

    addCondition(condition) {
        this.setState({
            conditions: this.state.conditions.concat(condition)
        })
    }

    removeCondition(id) {
        this.state.conditions.splice(id, 1);
        this.state.conditions.forEach(function (element, i) {
            element.Id = i;
        })
        this.setState({
            conditions: this.state.conditions
        })
    }

    render() {
        return (
            <div>
                <form>
                    <Webhook
                        name={this.state.name}
                        secondsBetweenCheck={this.state.secondsBetweenCheck}
                        sendAddress={this.state.sendAddress}
                        setName={this.setName}
                        setDemand={this.setDemand}
                        setSecondsBetweenCheck={this.setSecondsBetweenCheck}
                        setSendAddress={this.setSendAddress}
                    />
                    <Entity
                        apps={this.state.apps}
                        setApps={this.setApps}
                    />
                    <Condition
                        conditions={this.state.conditions}
                        addCondition={this.addCondition}
                        removeCondition={this.removeCondition}
                    />
                <input type="button" value="Submit" onClick={this.handleSubmit} />
                </form>
            </div>
        );
    }
}
