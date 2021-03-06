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
                var a = {};
                a.x = Math.min(shape.a.x, shape.b.x);
                a.y = Math.min(shape.a.y, shape.b.y);
                var w = Math.abs(shape.a.x - shape.b.x);
                var h = Math.abs(shape.a.y - shape.b.y);
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.moveTo(shape.a.x + 0.5, shape.a.y + 0.5);
                ctx.lineTo(shape.b.x + 0.5, shape.b.y + 0.5);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'RECTANGLE':
                var a = {};
                a.x = Math.min(shape.a.x, shape.b.x);
                a.y = Math.min(shape.a.y, shape.b.y);
                var w = Math.abs(shape.a.x - shape.b.x);
                var h = Math.abs(shape.a.y - shape.b.y);
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.fillStyle = brush(shape.fill, a, w, h);
                ctx.moveTo(shape.a.x + 0.5, shape.a.y + 0.5);
                ctx.lineTo(shape.b.x + 0.5, shape.a.y + 0.5);
                ctx.lineTo(shape.b.x + 0.5, shape.b.y + 0.5);
                ctx.lineTo(shape.a.x + 0.5, shape.b.y + 0.5);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'ELLIPSE':
                var a = {};
                a.x = Math.min(shape.a.x, shape.b.x);
                a.y = Math.min(shape.a.y, shape.b.y);
                var w = Math.abs(shape.a.x - shape.b.x);
                var h = Math.abs(shape.a.y - shape.b.y);
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.fillStyle = brush(shape.fill, a, w, h);
                ctx.ellipse((shape.a.x + shape.b.x) / 2, (shape.a.y + shape.b.y) / 2, Math.abs(shape.a.x - shape.b.x) / 2, Math.abs(shape.a.y - shape.b.y) / 2, 0, 0, 2 * Math.PI);
                ctx.closePath();
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'POLYLINE':
                var a = {};
                a.x = Infinity;
                a.y = Infinity;
                var b = {};
                b.x = -Infinity;
                b.y = -Infinity;
                for (var ii in shape.points) {
                    a.x = Math.min(a.x, shape.points[ii].x);
                    a.y = Math.min(a.y, shape.points[ii].y);
                    b.x = Math.max(b.x, shape.points[ii].x);
                    b.y = Math.max(b.y, shape.points[ii].y);
                }
                var w = b.x - a.x;
                var h = b.y - a.y;
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.fillStyle = brush(shape.fill, a, w, h);
                ctx.moveTo(shape.points[0].x + 0.5, shape.points[0].y + 0.5);
                for (var ii = 1; ii < shape.points.length; ii++) {
                    ctx.lineTo(shape.points[ii].x + 0.5, shape.points[ii].y + 0.5);
                }
                ctx.stroke();
                ctx.fill('evenodd');
                draw(i, j + 1);
                break;
            case 'PENCIL':
                var a = {};
                a.x = Infinity;
                a.y = Infinity;
                var b = {};
                b.x = -Infinity;
                b.y = -Infinity;
                for (var ii in shape.points) {
                    a.x = Math.min(a.x, shape.points[ii].x);
                    a.y = Math.min(a.y, shape.points[ii].y);
                    b.x = Math.max(b.x, shape.points[ii].x);
                    b.y = Math.max(b.y, shape.points[ii].y);
                }
                var w = b.x - a.x;
                var h = b.y - a.y;
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.moveTo(shape.points[0].x + 0.5, shape.points[0].y + 0.5);
                for (var ii = 1; ii < shape.points.length; ii++) {
                    ctx.lineTo(shape.points[ii].x + 0.5, shape.points[ii].y + 0.5);
                }
                ctx.stroke();
                ctx.fill('evenodd');
                draw(i, j + 1);
                break;
            case 'QLINE':
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.fillStyle = brush(shape.fill, a, w, h);
                ctx.beginPath();
                ctx.moveTo(shape.a.x + 0.5, shape.a.y + 0.5);
                ctx.quadraticCurveTo(shape.b.x + 0.5, shape.b.y + 0.5, shape.c.x + 0.5, shape.c.y + 0.5);
                ctx.stroke();
                ctx.fill();
                draw(i, j + 1);
                break;
            case 'TEXT':
                var imgData = ctx.getImageData(shape.a.x, shape.a.y, shape.w, shape.h);
                imgData = imageText(shape, imgData);
                ctx.putImageData(imgData, shape.a.x, shape.a.y);
                draw(i, j + 1);
                break;
            case 'POLYGON':
                var a = {};
                a.x = Infinity;
                a.y = Infinity;
                var b = {};
                b.x = -Infinity;
                b.y = -Infinity;
                for (var ii in shape.points) {
                    a.x = Math.min(a.x, shape.points[ii].x);
                    a.y = Math.min(a.y, shape.points[ii].y);
                    b.x = Math.max(b.x, shape.points[ii].x);
                    b.y = Math.max(b.y, shape.points[ii].y);
                }
                var w = b.x - a.x;
                var h = b.y - a.y;
                ctx.strokeStyle = brush(shape.stroke, a, w, h);
                ctx.fillStyle = brush(shape.fill, a, w, h);
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
                    ctx.drawImage(image, shape.a.x, shape.a.y);
                    draw(i, j + 1);
                };
                image.src = 'data:image/png;base64,' + shape.b64;
        }
    }
}

function brush(b, a, w, h) {
    if (b == null) return 'rgba(0,0,0,0)';
    switch (b.type) {
        case 'COLOR':
            return 'rgba(' + b.r + ',' + b.g + ',' + b.b + ',' + (b.a / 255.0).toString().replace(',', '.') + ')';
        case 'LG':
            var grd = ctx.createLinearGradient(a.x + b.s.x * w, a.y + b.s.y * h, a.x + b.e.x * w, a.y + b.e.y * h);
            for (var i in b.stops) {
                grd.addColorStop(b.stops[i].offset, brush(b.stops[i].color));
            }
            return grd;
        case 'RG':
            var grd = ctx.createRadialGradient(a.x + b.e.x * w, a.y + b.e.y * h, 0, a.x + b.e.x * w, a.y + b.e.y * h, w / 2);
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