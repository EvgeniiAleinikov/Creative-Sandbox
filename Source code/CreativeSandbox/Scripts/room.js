$(function () { 
    var img = new Image();

    var drawGame = {
        isDrawing: false,
        startX: 0,
        startY: 0,
    };

    var data = {
        startX: 0,
        startY: 0,
        endX: 0,
        endY: 0
    };

    var canvas = document.getElementById('drawingpad');
    var ctx = canvas.getContext('2d');

    var chat = $.connection.roomHub;

    $('#moveTool').click(function () {
        $('canvas').css("z-index", "-1");
    });

    $('#paintTool').click(function () {
        $('canvas').css("z-index", "10");
    });

    $('#clearTool').click(function () {
        chat.client.clearAll();
        chat.server.clearAll($("#textId")[0].innerText.substring(4));
    });

    chat.client.clearAll = function () {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        $("[id*=wrap]").remove();
    };

    chat.client.addLine = function (data) {
        drawLine(ctx, data.startX, data.startY, data.endX, data.endY, 1);
    };

    chat.client.loadImage = function (canvasUrl) {
        img.onload = function () {
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
        };
        img.crossOrigin = '*';
        img.src = canvasUrl;
    };

    function drawLine(ctx, x1, y1, x2, y2, thickness) {
        ctx.beginPath();
        ctx.moveTo(x1, y1);
        ctx.lineTo(x2, y2);
        ctx.lineWidth = 2;
        ctx.strokeStyle = "#449";
        ctx.stroke();
    }

    function mousedown(e) {
        var mouseX = e.layerX || 0;
        var mouseY = e.layerY || 0;
        drawGame.startX = mouseX;
        drawGame.startY = mouseY;
        drawGame.isDrawing = true;
    };


    window.onbeforeunload = function () {
        chat.server.leaveRoom($("#textId")[0].innerText.substring(4));
    }


    function mousemove(e) {
        if (drawGame.isDrawing) {
            var mouseX = e.layerX || 0;
            var mouseY = e.layerY || 0;
            if (!(mouseX == drawGame.startX &&
                mouseY == drawGame.startY)) {
                drawLine(ctx, drawGame.startX,
                    drawGame.startY, mouseX, mouseY, 1);

                data.startX = drawGame.startX;
                data.startY = drawGame.startY;
                data.endX = mouseX;
                data.endY = mouseY;

                chat.server.send(data, $("#textId")[0].innerText.substring(4));

                drawGame.startX = mouseX;
                drawGame.startY = mouseY;
            }
        }
    };

    function mouseup(e) {
        drawGame.isDrawing = false;
        sendDrawSpace();
    }

    sendDrawSpace = function () {
        uploadFile(dataURItoBlob(canvas.toDataURL('image/png')), 'room' + $("#textId")[0].innerText.substring(4) + '.png', true);
    }

    function dataURItoBlob(dataURI) {
        var byteString;
        if (dataURI.split(',')[0].indexOf('base64') >= 0)
            byteString = atob(dataURI.split(',')[1]);
        else
            byteString = unescape(dataURI.split(',')[1]);

        var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

        var ia = new Uint8Array(byteString.length);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }

        return new Blob([ia], { type: mimeString });
    }

    var dropZone = $('#imageDropZone');

    dropZone[0].ondragover = function () {
        dropZone.addClass('hover');
        return false;
    };

    dropZone[0].ondragleave = function () {
        dropZone.removeClass('hover');
        return false;
    };

    var imgId = 0;

    dropZone[0].ondrop = function (event) {
        event.preventDefault();
        dropZone.removeClass('hover');
        dropZone.addClass('drop');

        var imageId = imgId;
        var result = uploadFile(event.dataTransfer.files[0], event.dataTransfer.files[0].name, false);
        chat.client.addImage(result, imageId);
        chat.server.addImage($("#textId")[0].innerText.substring(4), $('#im' + imageId).attr('src'), imageId);
    };

    uploadFile = function (file, fileName, asuncUpload) {
        var data = new FormData();
        data.append("file", file, fileName);

        return $.ajax({
            type: "POST",
            url: '/Home/Upload',
            contentType: false,
            processData: false,
            data: data,
            async: asuncUpload,
        }).responseJSON;
    }

    deleteImg = function (imageId) {
        chat.server.deleteImage($("#textId")[0].innerText.substring(4), imageId);
        chat.client.deleteImage(imageId);
    }

    chat.client.deleteImage = function (imageId) {
        $('#wrap' + imageId).remove();
    };

    chat.client.addImage = function (imageUrl, imageId) {
        imgId = imageId + 1;
        $('#imageDropZone').append("<div style='top:5px; left:5px; position:absolute;' id='wrap" + imageId + "'><img id='im" + imageId + "' src='" + imageUrl + "' style='height: 100%; width: 100%;' /><div onclick='deleteImg(" + imageId + ");' class='trash-button glyphicon glyphicon-trash'></div></div>");
        $('#wrap' + imageId).resizable({
            containment: "canvas",
            resize: function () {
                chat.server.resizeImage($("#textId")[0].innerText.substring(4), this.id.substring(4), this.style.width, this.style.height);
            }
        });
        $('#wrap' + imageId).draggable({
            containment: "canvas", zIndex: 7500,
            drag: function () {
                chat.server.dragImage($("#textId")[0].innerText.substring(4), this.id.substring(4), this.style.top, this.style.left);
            }
        });
        $('#wrap' + imageId).rotatable({
            containment: "canvas",
            rotate: function () {
                chat.server.rotateImage($("#textId")[0].innerText.substring(4), this.id.substring(4), this.style.transform);
            }
        });
    };

    chat.client.dragImage = function (imageId, top, left) {
        $('#wrap' + imageId).css("left", left).css("top", top);
    }

    chat.client.resizeImage = function (imageId, width, height) {
        $('#wrap' + imageId).css("width", width).css("height", height);
    }

    chat.client.rotateImage = function (imageId, transform) {
        $('#wrap' + imageId).css("transform", transform);
    }

    $.connection.hub.start().done(function () {
        chat.server.connectToRoom($("#textId")[0].innerText.substring(4));

        canvas.addEventListener("mousedown", mousedown, false);
        canvas.addEventListener("mousemove", mousemove, false);
        canvas.addEventListener("mouseup", mouseup, false);
    });
});

