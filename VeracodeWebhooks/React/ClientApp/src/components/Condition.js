import React, { Component } from 'react';
import AsyncSelect from 'react-select/async';
import "react-datepicker/dist/react-datepicker.css";

class Condition extends Component {
    constructor(props) {
        super(props);
        this.state = {
            field: null,
            isAction: false,
            expected: null
        };

        this.handleFieldListChange = this.handleFieldListChange.bind(this);
        this.actionTypeSelected = this.actionTypeSelected.bind(this);
        this.getMitigationFields = this.getMitigationFields.bind(this);
        this.getActionTypes = this.getActionTypes.bind(this);
        this.textTypeChanged = this.textTypeChanged.bind(this);
        this.get = this.get.bind(this);
        this.submitCondition = this.submitCondition.bind(this);
    }

    handleFieldListChange(event) {
        this.setState({ isAction: (event.value === "Action") });
        this.setState({ field: event.value });    
    }

    actionTypeSelected(event) {
        this.setState({ expected: event.value });
        this.setState({ description: event.label });
    }

    textTypeChanged(event) {
        this.setState({ expected: event.target.value });
        this.setState({ description: event.target.value });
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

    getMitigationFields() {
        return this.get(`Condition/GetMitigationActionFields`);
    }   

    getActionTypes() {
        return this.get(`Condition/GetActionType`);
    }  

    submitCondition() {
        this.props.addCondition({
            Field: this.state.field,
            ExpectedValue: this.state.expected,
            Description: this.state.description
        })
    }

    removeCondition(event) {
        this.props.removeCondition(event.target.id);
    }

    render() {
        return (
            <div>
                <label htmlFor="name">You can add conditions here</label>
                <div className="form-group">
                    <AsyncSelect
                        cacheOptions
                        defaultOptions
                        onChange={this.handleFieldListChange}
                        loadOptions={this.getMitigationFields}
                    />
                </div>

                <div style={{ display: this.state.isAction ? 'block' : 'none' }}>
                    <AsyncSelect
                        cacheOptions
                        defaultOptions
                        onChange={this.actionTypeSelected}
                        loadOptions={this.getActionTypes}
                    />
                </div>

                <div style={{ display: this.state.isAction ? 'none' : 'block' }}>
                    <input
                        name="expected"
                        ref="expected"
                        type="text"
                        onChange={this.textTypeChanged}
                    />
                </div>

                <button
                    type="button"
                    disabled={this.state.expected == null}
                    onClick={this.submitCondition}
                >+</button>


                <div className="form-group">
                    <label>Current Conditions</label>
                    <table className='table table-striped' aria-labelledby="tabelLabel">
                        <thead>
                            <tr>
                                <th>Field</th>
                                <th>Expected</th>
                                <th>Remove</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.props.conditions.map(condition =>
                                <tr key={condition.Id}>
                                    <td>{condition.Field} is equal to</td>
                                    <td>{condition.Description}</td>
                                    <td><button
                                        id={condition.Id}
                                        type="button"
                                        onClick={this.removeCondition.bind(this)}
                                    >-</button></td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }
}
export default Condition;

