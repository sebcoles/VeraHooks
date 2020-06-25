# VeraHooks

VeraHooks is solution that enables webhooks against mitigations within the Veracode platform. A webhook is configured against 1 or more application profiles and when the webhook meets the desired conditions (such as "Mitigate by Design") a webhook can be fired to an address of your choice.

## Solution
The solution has 2 components, the portal and the agents.

### Portal
The portal (.NET CORE/React) is where webhooks can be configured against an application profiles. 

You must give the webhook a:
- Title to describe the webhook
- A web endpoint to POST too
- A "frequency to check" for example do you want the webhook to check once a day, or every hour etc
- Which agent to run the job against
- Conditions, such as the state of the mitigation (Mitigate by Design, Comment, etc)

The conditions allow for workflows such as sending a mitigation abuot the Network envirnoment to a network team or an OS mitigation to an OS team etc.

The portal can also be used to delete webhooks, check the logs of fired webhooks and see the status of the agents.

### Agents
These are .NET Core Console agents that periodically check to see if the conditions of a webhook have been met. They then fire the webhook to the configured location. They qill periodically call the portal to provide a status, such as a connection to Veracode. 

The reason for taking an agent based approach is that the XML APIs can be relatively slow. Depending on the size of your estate and the volume of flaws, it can take seconds for an app to be polled. This approach allows as many agents as you need, and you have then assign a couple of application profiles to each agent. They can also be configured with seperate API credentials.

## Pre Reqs
This solution relies on a MS SQL Server database to store webhook information.

The VeracodeServicesCore NuGet wrappers requires your Veracode API credentials. Please view Veracode help to find out how to get your credentials 
[how to get your credentials from Veracode Platform](https://help.veracode.com/reader/JVdG5ruGOiJnRpaJmQVCSQ/CzrWjLoJABEwD1Tozaqciw).
