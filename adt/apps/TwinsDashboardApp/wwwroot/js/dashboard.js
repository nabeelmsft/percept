"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dashboardHub").build();


connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.on("ReceiveTwinMessage", function (floorId, floorName, label, confidence, timeStamp) {
    console.log("In ReceiveTwinMessage");
    var greencolor = 255 * `${ confidence }`;
    document.getElementById("labelId").style.background = `rgba(0, ${greencolor}, 0, 1)`;
    document.getElementById("floorIdId").innerText = `${floorId}`;
    document.getElementById("floorNameId").innerText = `${floorName}`;
    document.getElementById("labelId").innerText = `${label}`;
    document.getElementById("labelId").removeAttribute("class");
    document.getElementById("labelId").classList.add(`${label}`);
    document.getElementById("confidenceId").innerText = `${confidence}`;
});

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
