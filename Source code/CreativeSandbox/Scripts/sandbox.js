$(function() {

    var chat = $.connection.sandBoxHub;
    var chat1 = $.connection.roomHub;

    chat.client.changeRoomName = function(id, name) {
        $(".form-control").filter("#" + id).val(name);
    };

    chat.client.deleteRoom = function(id) {
        document.getElementById(id).remove();
    };

    chat.client.erorDeleteRoom = function(id, name) {
        alert("Sory, but you can't delete this room now. Try it later.")
    };

    chat.client.updateUsersInRoom = function(id, usersIn) {
        $(".usersIn").filter("#" + id).text(usersIn);
    };

    chat1.client.updateUsersInRoom = function(id, usersIn) {
        $(".usersIn").filter("#" + id).text(usersIn);
    };

    requestDeleteRoom = function(id) {
        chat.server.deleteRoom(id);
    };

    chat.client.receiveRoom = function(id, name, usersIn) {
        var roomRow = document.getElementById("room");

        var roomNode = document.createElement("div");
        roomNode.setAttribute("class", "row room-row");
        roomNode.setAttribute("id", id);
        roomNode.setAttribute("style", "margin-top:10px;");

        var idNode = document.createElement("div");
        idNode.setAttribute("class", "col-lg-1 col-md-1 col-sm-1 col-xs-1 text-center")
        idNode.appendChild(document.createTextNode("id: " + id))

        var formNode = document.createElement("div");
        formNode.setAttribute("class", "col-lg-8 col-md-8 col-sm-8 col-xs-8")
        formNode.setAttribute("style", "padding:10px;");

        var inputFormNode = document.createElement("input");
        inputFormNode.setAttribute("class", "form-control");
        inputFormNode.setAttribute("value", name);
        inputFormNode.setAttribute("id", id);
        inputFormNode.onchange = function() {
            chat.server.changeRoomName(this.id, this.value);
        };

        formNode.appendChild(inputFormNode);

        var usersInNode = document.createElement("div");
        usersInNode.setAttribute("class", "col-lg-1 col-md-1 col-sm-1 col-xs-1 usersIn text-center")
        usersInNode.setAttribute("id", id)
        usersInNode.appendChild(document.createTextNode(usersIn))

        var enterNode = document.createElement("div");
        enterNode.setAttribute("class", "col-lg-1 col-md-1 col-sm-1 col-xs-1");

        var enterButtonNode = document.createElement("button");
        enterButtonNode.setAttribute("onclick", "location.href ='Room?id=" + id + "'");
        enterButtonNode.setAttribute("class", "btn btn-enter");
        enterButtonNode.setAttribute("id", id)
        enterButtonNode.appendChild(document.createTextNode("Enter"));

        enterNode.appendChild(enterButtonNode);

        var deleteNode = document.createElement("div");
        deleteNode.setAttribute("class", "col-lg-1 col-md-1 col-sm-1 col-xs-1");

        var deleteButtonNode = document.createElement("button");
        deleteButtonNode.setAttribute("onclick", "requestDeleteRoom(" + id + ");");
        deleteButtonNode.setAttribute("class", "btn btn-danger");
        deleteButtonNode.setAttribute("id", id)
        deleteButtonNode.appendChild(document.createTextNode("X"));

        deleteNode.appendChild(deleteButtonNode);

        roomNode.appendChild(idNode);
        roomNode.appendChild(formNode);
        roomNode.appendChild(usersInNode);
        roomNode.appendChild(enterNode);
        roomNode.appendChild(deleteNode);

        roomRow.appendChild(roomNode);
    };

    $.connection.hub.start().done(function() {
        $('#addRoomButton').click(function() {
            chat.server.addRoom();
        });

        $('.form-control').change(function() {
            chat.server.changeRoomName(this.id, this.value);
        });
    });
});