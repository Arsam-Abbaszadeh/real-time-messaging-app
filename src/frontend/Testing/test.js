const signalR = require("@microsoft/signalr");
const tempAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJSZWFsVGltZU1lc3NhZ2luZ1dlYkFwcFVzZXJzIiwiaXNzIjoiUmVhbFRpbWVNZXNzYWdpbmdXZWJBcHAiLCJleHAiOjE3Njc2NzEwNzYsImlkIjoiYTY2NjM0MjktZjZiNy00NGY4LTlkNjEtMDg2MDM2YzUzMjZlIiwidXNlcm5hbWUiOiJ0ZXN0MiIsImlhdCI6MTc2NzA3MTA3NiwibmJmIjoxNzY3MDcxMDc2fQ.Qy7Hkj7ZXcExSYvjB13nOG1MOhoUbDGi2dcS88Nreyk"


const connection = new signalR.HubConnectionBuilder()
    //  might need to specify connection type like web socket or whatever
    .withUrl("http://localhost:5231/chathub", {
        accessTokenFactory: () => tempAccessToken
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 2000);
    }
};

connection.onclose(async () => {
    await start();
});

// Start the connection.
start();
