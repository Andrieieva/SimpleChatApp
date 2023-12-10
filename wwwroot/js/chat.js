document.addEventListener("DOMContentLoaded", function () {

    var storedUserName = localStorage.getItem("userName");
    var defaultUserName = storedUserName || "Guest";

    var userInput = document.getElementById("userInput");
    userInput.value = defaultUserName;

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    connection.start().then(function () {
        connection.invoke("GetChatHistory").catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("sendButton").addEventListener("click", function () {
        var user = userInput.value;
        var message = document.getElementById("messageInput").value;

        localStorage.setItem("userName", user);

        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });

        document.getElementById("messageInput").value = "";
    });

    connection.on("ReceiveMessage", function (user, message) {
        var encodedMessage = user + " says: " + message;
        var li = document.createElement("li");
        li.textContent = encodedMessage;
        document.getElementById("messagesList").appendChild(li);
    });

    connection.on("UserConnected", function (user) {
        var encodedMessage = user + " has joined the chat";
        var li = document.createElement("li");
        li.textContent = encodedMessage;
        document.getElementById("messagesList").appendChild(li);
    });

    connection.on("LoadChatHistory", function (chatHistory) {
        chatHistory.forEach(function (item) {
            var encodedMessage = item.user + " says: " + item.text;
            var li = document.createElement("li");
            li.textContent = encodedMessage;
            document.getElementById("messagesList").appendChild(li);
        });
    });
});
