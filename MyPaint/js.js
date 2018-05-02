function draw(i, j) {
    if (json.layers.length <= i) return;
    if (j == -1) {

        if (!json.layers[i].visible) {
            i++;
        } else {
            ctx.beginPath();
            ctx.moveTo(0.5, 0.5);
            ctx.rect(0.5, 0.5, json.resolution.x + 0.5, json.resolution.y + 0.5);
            ctx.fillStyle = brush(json.layers[i].color, { x: 0, y: 0 }, json.resolution.x, json.resolution.y);
            ctx.fill();
        }
        draw(i, 0);
    } else {
        if (json.layers[i].shapes.length <= j) {
            draw(i + 1, -1);
            return;
        }
        var shape = json.layers[i].shapes[j];
        ctx.beginPath();
        ctx.lineWidth = shape.lineWidth;
        switch (shape.type) {
            case 'LINE':
                var A = {};
                A.x = Math.min(shape.A.x, shape.B.x);
                A.y = Math.min(shape.A.y, shape.B.y);
                var w = Math.abs(shape.A.x - shape.B.x);
                var h = Math.abs(shape.A.y - shape.B.y);
                ctx.strokeStyle = brush(shape.stroke, A, w, h);
                ctx.moveTo(shape.A.x + 0.5, shape.A.y + 0.5);
                ctx.lineTo(shape.B.x + 0.5, shape.B.y + 0.5);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'RECTANGLE':
                var A = {};
                A.x = Math.min(shape.A.x, shape.B.x);
                A.y = Math.min(shape.A.y, shape.B.y);
                var w = Math.abs(shape.A.x - shape.B.x);
                var h = Math.abs(shape.A.y - shape.B.y);
                ctx.strokeStyle = brush(shape.stroke, A, w, h);
                ctx.fillStyle = brush(shape.fill, A, w, h);
                ctx.moveTo(shape.A.x + 0.5, shape.A.y + 0.5);
                ctx.lineTo(shape.B.x + 0.5, shape.A.y + 0.5);
                ctx.lineTo(shape.B.x + 0.5, shape.B.y + 0.5);
                ctx.lineTo(shape.A.x + 0.5, shape.B.y + 0.5);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'ELLIPSE':
                var A = {};
                A.x = Math.min(shape.A.x, shape.B.x);
                A.y = Math.min(shape.A.y, shape.B.y);
                var w = Math.abs(shape.A.x - shape.B.x);
                var h = Math.abs(shape.A.y - shape.B.y);
                ctx.strokeStyle = brush(shape.stroke, A, w, h);
                ctx.fillStyle = brush(shape.fill, A, w, h);
                ctx.ellipse((shape.A.x + shape.B.x) / 2, (shape.A.y + shape.B.y) / 2, Math.abs(shape.A.x - shape.B.x) / 2, Math.abs(shape.A.y - shape.B.y) / 2, 0, 0, 2 * Math.PI);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'POLYLINE':
                var A = {};
                A.x = Infinity;
                A.y = Infinity;
                var B = {};
                B.x = -Infinity;
                B.y = -Infinity;
                for (var ii in shape.points) {
                    A.x = Math.min(A.x, shape.points[ii].x);
                    A.y = Math.min(A.y, shape.points[ii].y);
                    B.x = Math.max(B.x, shape.points[ii].x);
                    B.y = Math.max(B.y, shape.points[ii].y);
                }
                var w = B.x - A.x;
                var h = B.y - A.y;
                ctx.strokeStyle = brush(shape.stroke, A, w, h);
                ctx.fillStyle = brush(shape.fill, A, w, h);
                ctx.moveTo(shape.points[0].x + 0.5, shape.points[0].y + 0.5);
                for (var ii = 1; ii < shape.points.length; ii++) {
                    ctx.lineTo(shape.points[ii].x + 0.5, shape.points[ii].y + 0.5);
                }
                ctx.stroke();
                ctx.fill('evenodd');
                draw(i, j + 1);
                break;
            case 'QLINE':
                ctx.strokeStyle = brush(shape.stroke, A, w, h);
                ctx.fillStyle = brush(shape.fill, A, w, h);
                ctx.beginPath();
                ctx.moveTo(shape.A.x + 0.5, shape.A.y + 0.5);
                ctx.quadraticCurveTo(shape.B.x + 0.5, shape.B.y + 0.5, shape.C.x + 0.5, shape.C.y + 0.5);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'TEXT':
                var imgData = ctx.getImageData(shape.A.x, shape.A.y, shape.w, shape.h);
                imgData = imageText(shape, imgData);
                ctx.putImageData(imgData, shape.A.x, shape.A.y);
                draw(i, j + 1);
                break;
            case 'POLYGON':
                var A = {};
                A.x = Infinity;
                A.y = Infinity;
                var B = {};
                B.x = -Infinity;
                B.y = -Infinity;
                for (var ii in shape.points) {
                    A.x = Math.min(A.x, shape.points[ii].x);
                    A.y = Math.min(A.y, shape.points[ii].y);
                    B.x = Math.max(B.x, shape.points[ii].x);
                    B.y = Math.max(B.y, shape.points[ii].y);
                }
                var w = B.x - A.x;
                var h = B.y - A.y;
                ctx.strokeStyle = brush(shape.stroke, A, w, h);
                ctx.fillStyle = brush(shape.fill, A, w, h);
                ctx.moveTo(shape.points[0].x + 0.5, shape.points[0].y + 0.5);
                for (var ii = 1; ii < shape.points.length; ii++) {
                    ctx.lineTo(shape.points[ii].x + 0.5, shape.points[ii].y + 0.5);
                }
                ctx.closePath();
                ctx.stroke();
                ctx.fill('evenodd');
                draw(i, j + 1);
                break;
            case 'IMAGE':
                var image = new Image();
                image.onload = function () {
                    ctx.moveTo(0.5, 0.5);
                    ctx.drawImage(image, shape.A.x, shape.A.y);
                    draw(i, j + 1);
                };
                image.src = 'data:image/png;base64,' + shape.b64;
        }
    }
}

function brush(b, A, w, h) {
    if (b == null) return 'rgba(0,0,0,0)';
    switch (b.type) {
        case 'COLOR':
            return 'rgba(' + b.R + ',' + b.G + ',' + b.B + ',' + (b.A / 255.0).toString().replace(',', '.') + ')';
        case 'LG':
            var grd = ctx.createLinearGradient(A.x + b.S.x * w, A.y + b.S.y * h, A.x + b.E.x * w, A.y + b.E.y * h);
            for (var i in b.stops) {
                grd.addColorStop(b.stops[i].offset, brush(b.stops[i].color));
            }
            return grd;
        case 'RG':
            var grd = ctx.createRadialGradient(A.x + b.E.x * w, A.y + b.E.y * h, 0, A.x + b.E.x * w, A.y + b.E.y * h, w / 2);

            for (var i in b.stops) {
                grd.addColorStop(b.stops[i].offset, brush(b.stops[i].color));
            }
            return grd;
        case 'NULL':
            return 'rgba(0,0,0,0)';
    }
}

function imageText(shape, image) {
    var canvas = document.createElement('canvas');
    canvas.width = shape.w;
    canvas.height = shape.h;
    var ctx = canvas.getContext('2d');
    ctx.putImageData(image, 0, 0);
    ctx.beginPath();
    ctx.moveTo(0, 0);
    ctx.rect(0, 0, shape.w, shape.h);
    ctx.fillStyle = brush(shape.fill, { x: 0, y: 0 }, shape.w, shape.h);
    ctx.fill();
    ctx.textBaseline = 'top';
    ctx.font = shape.lineWidth + "px " + shape.font;
    ctx.fillStyle = brush(shape.stroke, { x: 0, y: 0 }, shape.w, shape.h);
    var lines = shape.b64.split('\n');
    for (var i = 0; i < lines.length; i++) {
        ctx.fillStyle = brush(shape.stroke, { x: 0, y: shape.lineWidth * 1.15 * i }, ctx.measureText(lines[i]).width, shape.lineWidth * 1.15);
        ctx.fillText(lines[i], 0, 0 + (i * (shape.lineWidth * 1.15)));
    }
    return ctx.getImageData(0, 0, shape.w, shape.h);
}