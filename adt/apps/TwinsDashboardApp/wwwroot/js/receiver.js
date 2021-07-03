"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dashboardHub").build();


connection.start().then(function () {
    var url = window.location.toString();
    var queryString = url.substring(url.indexOf('?') + 1);
    var queryStringParameters = parseQueryString(queryString);
    console.log(queryStringParameters);

    console.log("in receiver");
    console.log(connection);
    console.log(queryStringParameters.floorId, queryStringParameters.floorName, queryStringParameters.label, queryStringParameters.confidence, queryStringParameters.timestamp);
    connection.invoke("SendTwinMessage", queryStringParameters.floorId, queryStringParameters.floorName, queryStringParameters.label, queryStringParameters.confidence, queryStringParameters.timestamp).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}).catch(function (err) {
    return console.error(err.toString());
});

var parseQueryString = function (queryString) {
    var params = {}, queries, temp, i, l;

    // Split into key/value pairs
    queries = queryString.split("&");

    // Convert the array of strings into an object
    for (i = 0, l = queries.length; i < l; i++) {
        temp = queries[i].split('=');
        params[temp[0]] = temp[1];
    }

    return params;
};

