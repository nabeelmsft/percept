"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dashboardHub").build();

connection.on("ReceiveTwinMessage", function (floorId, floorName, label, confidence, timeStamp, rawMessage) {
    var greencolor = 255 * `${ confidence }`;
    document.getElementById("labelId").style.background = `rgba(0, ${greencolor}, 0, 1)`;
    document.getElementById("floorIdId").innerText = `FloorId: ${floorId}`;
    document.getElementById("floorNameId").innerText = `FloorName: ${floorName}`;
    document.getElementById("labelId").innerText = `${label}`;
    document.getElementById("labelId").removeAttribute("class");
    document.getElementById("labelId").classList.add(`${label}`);
    document.getElementById("confidenceId").innerText = `Confidence: ${confidence}`;
    document.getElementById("messageArea").value = rawMessage;
});

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
