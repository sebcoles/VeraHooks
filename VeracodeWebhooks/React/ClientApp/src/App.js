import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './Layout';
import { Add } from './views/Add';
import { List } from './views/List';
import { Agents } from './views/Agents';
import { Log } from './views/Log';
import { ToastContainer } from 'react-toastify';

import 'react-toastify/dist/ReactToastify.css';
import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Add} />
        <Route path='/list' component={List} />
        <Route path='/agents' component={Agents} />
        <Route path='/logs' component={Log} />
            <ToastContainer
                position="top-right"
                autoClose={1500}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover />
      </Layout>
    );
  }
}
