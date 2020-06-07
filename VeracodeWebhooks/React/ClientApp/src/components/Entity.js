import React, { Component } from 'react';
import AsyncSelect from 'react-select/async';
import "react-datepicker/dist/react-datepicker.css";

class Entity extends Component {
    constructor(props) {
        super(props);
        this.state = {
            applist: [],
        };

        this.setApps = this.setApps.bind(this);
        this.getApplications = this.getApplications.bind(this);
        this.get = this.get.bind(this);
    }

    setApps(event) {
        this.props.setApps(event.map(x => ({
            appid: parseInt(x.value),
            appname: x.label
        })));
    }

    getApplications() {
        return this.get('Veracode/Applications');
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

    render() {
        return (          
            <div>
                <div className="form-group">
                <label htmlFor="name">Which applications are you watching?</label>
                <AsyncSelect
                    isMulti
                    cacheOptions
                    defaultOptions
                    onChange={this.setApps}
                    loadOptions={this.getApplications}
                />               
            </div>
            </div>
        );
    }
}
export default Entity;

