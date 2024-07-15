
"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer")
    .build();

connection.on("ReceiveCustomerUpdate", function () {
    location.href = '/Admin/Customers/Index';
});
connection.on("ReceiveProductUpdate", function () {
    location.href = '/Admin/Rooms/Index';
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});