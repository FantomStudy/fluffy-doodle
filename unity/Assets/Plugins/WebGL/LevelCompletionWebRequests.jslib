mergeInto(LibraryManager.library, {
  PostLevelCompletionWebRequest: function (gameObjectNamePtr, successCallbackNamePtr, errorCallbackNamePtr, urlPtr, bodyJsonPtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var successCallbackName = UTF8ToString(successCallbackNamePtr);
    var errorCallbackName = UTF8ToString(errorCallbackNamePtr);
    var url = UTF8ToString(urlPtr);
    var bodyJson = UTF8ToString(bodyJsonPtr);

    fetch(url, {
      method: "POST",
      credentials: "include",
      headers: {
        "Accept": "application/json",
        "Content-Type": "application/json"
      },
      body: bodyJson
    })
      .then(function (response) {
        return response.text().then(function (responseText) {
          if (response.ok) {
            SendMessage(gameObjectName, successCallbackName, responseText || "");
            return;
          }

          var errorText = "HTTP " + response.status;
          if (responseText) {
            errorText += ": " + responseText;
          }

          SendMessage(gameObjectName, errorCallbackName, errorText);
        });
      })
      .catch(function (error) {
        var errorText = error && error.message ? error.message : "Unknown WebGL fetch error.";
        SendMessage(gameObjectName, errorCallbackName, errorText);
      });
  }
});
