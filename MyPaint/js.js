for (var j in json) {
    var shape = json[j];
    ctx.beginPath();
    ctx.lineWidth = shape.lineWidth;
    switch (shape.type) {
        case 'LINE':
            ctx.strokeStyle = brush(shape.stroke);
            ctx.moveTo(shape.A.x, shape.A.y);
            ctx.lineTo(shape.B.x, shape.B.y);
            break;
        case 'RECTANGLE':
            ctx.strokeStyle = brush(shape.stroke);
            ctx.fillStyle = brush(shape.fill);
            ctx.moveTo(shape.A.x, shape.A.y);
            ctx.lineTo(shape.B.x, shape.A.y);
            ctx.lineTo(shape.B.x, shape.B.y);
            ctx.lineTo(shape.A.x, shape.B.y);
            break;
        case 'ELLIPSE':
            ctx.strokeStyle = brush(shape.stroke);
            ctx.fillStyle = brush(shape.fill);
            ctx.ellipse((shape.A.x + shape.B.x) / 2, (shape.A.y + shape.B.y) / 2, Math.abs(shape.A.x - shape.B.x) / 2, Math.abs(shape.A.y - shape.B.y) / 2, 0, 0, 2 * Math.PI);
            break;
        case 'POLYGON':
            ctx.strokeStyle = brush(shape.stroke);
            ctx.fillStyle = brush(shape.fill);
            ctx.moveTo(shape.points[0].x, shape.points[0].y);
            for (var i = 1; i < shape.points.length; i++) {
                console.log(1);
                ctx.lineTo(shape.points[i].x, shape.points[i].y);
            }
            break;
    }
    ctx.closePath();
    ctx.stroke();
    ctx.fill();
}

function brush(b) {
    if (b == null) return 'rgba(0,0,0,0)';
    switch (b.type) {
        case 'COLOR':
            return 'rgba(' + b.R + ',' + b.G + ',' + b.B + ',' + (b.A / 255.0).toString().replace(',', '.') + ')';
    }
}