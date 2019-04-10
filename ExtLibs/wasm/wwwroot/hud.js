var deg2rad = Math.PI / 180;
var rad2deg = 180 / Math.PI;

var pitch = 70;
var roll = 20;
var yaw = 200;

function draw() {
    setTimeout("draw()", 250);

    //pitch -= 1.2;
    //roll += .75;

    //if (pitch < -90)
    //pitch = 90;
    //if (roll > 180)
    //roll = -180;

    var canvas = document.getElementById("canvas");
    if (canvas.getContext) {
        var ctx = canvas.getContext("2d");

        ctx.save();

        ctx.translate(canvas.width / 2, canvas.height / 2);

        ctx.rotate(-roll * deg2rad);

        var font = "Arial";
        var fontsize = canvas.height / 30;
        var fontoffset = fontsize - 10;

        var halfwidth = canvas.width / 2;
        var halfheight = canvas.height / 2;

        var every5deg = -canvas.height / 60;

        var pitchoffset = -pitch * every5deg;

        var x = Math.sin(-roll * deg2rad);
        var y = Math.cos(-roll * deg2rad);

        gradObj = ctx.createLinearGradient(0, -halfheight * 2, 0, halfheight * 2);
        gradObj.addColorStop(0.0, "Blue");
        var offset = 0.5 + pitchoffset / canvas.height / 2;
        if (offset < 0) {
            offset = 0;
        }
        if (offset > 1) {
            offset = 1;
        }
        gradObj.addColorStop(offset, "LightBlue");
        gradObj.addColorStop(offset, "#9bb824");
        gradObj.addColorStop(1.0, "#414f07");

        ctx.fillStyle = gradObj;
        ctx.rect(-halfwidth * 2, -halfheight * 2, halfwidth * 4, halfheight * 4);
        ctx.fill();

        var lengthshort = canvas.width / 12;
        var lengthlong = canvas.width / 8;

        for (var a = -90; a <= 90; a += 5) {
            // limit to 40 degrees
            if (a >= pitch - 34 && a <= pitch + 25) {
                if (a % 10 == 0) {
                    if (a == 0) {
                        DrawLine(ctx, "White", 4, canvas.width / 2 - lengthlong - halfwidth, pitchoffset + a * every5deg, canvas.width / 2 + lengthlong - halfwidth, pitchoffset + a * every5deg);
                    }
                    else {
                        DrawLine(ctx, "White", 4, canvas.width / 2 - lengthlong - halfwidth, pitchoffset + a * every5deg, canvas.width / 2 + lengthlong - halfwidth, pitchoffset + a * every5deg);
                    }
                    drawstring(ctx, a, font, fontsize + 2, "White", canvas.width / 2 - lengthlong - 30 - halfwidth - (fontoffset * 1.7), pitchoffset + a * every5deg - 8 - fontoffset);
                }
                else {
                    DrawLine(ctx, "White", 4, canvas.width / 2 - lengthshort - halfwidth, pitchoffset + a * every5deg, canvas.width / 2 + lengthshort - halfwidth, pitchoffset + a * every5deg);
                }
            }
        }

        lengthlong = canvas.height / 66;

        var extra = canvas.height / 15 * 4.9;

        var lengthlongex = lengthlong + 2;

        var pointlist = new Array();
        pointlist[0] = 0;
        pointlist[1] = -lengthlongex * 2 - extra;
        pointlist[2] = -lengthlongex;
        pointlist[3] = -lengthlongex - extra;
        pointlist[4] = lengthlongex;
        pointlist[5] = -lengthlongex - extra;

        DrawPolygon(ctx, "RED", 4, pointlist)

        for (var a = -60; a <= 60; a += 15) {
            ctx.restore();
            ctx.save();
            ctx.translate(canvas.width / 2, canvas.height / 2);
            ctx.rotate(a * deg2rad);
            drawstring(ctx, a.toString(), font, fontsize, "White", 0 - 6 - fontoffset, -lengthlong * 8 - extra + 10);
            DrawLine(ctx, "White", 4, 0, -lengthlong * 3 - extra, 0,
                -lengthlong * 3 - extra - lengthlong);
        }

        ctx.restore();
        ctx.save();

        DrawEllipse(ctx, "red", 4, halfwidth - 10, halfheight - 10, 20, 20);
        DrawLine(ctx, "red", 4, halfwidth - 10 - 10, halfheight, halfwidth - 10, halfheight);
        DrawLine(ctx, "red", 4, halfwidth - 10 + 20, halfheight, halfwidth - 10 + 20 + 10, halfheight);
        DrawLine(ctx, "red", 4, halfwidth - 10 + 20 / 2, halfheight - 10, halfwidth - 10 + 20 / 2, halfheight - 10 - 10);

        ///////////////////////

        var headbg = { Left: 0, Top: 0, Width: canvas.width - 0, Height: canvas.height / 14, Bottom: canvas.height / 14, Right: canvas.width - 0 };

        _targetheading = yaw;
        _heading = yaw;
        _groundcourse = yaw;

        DrawRectangle(ctx, "black", headbg);

        //FillRectangle(ctx,"", headbg);

        //bottom line
        DrawLine(ctx, "white", 2, headbg.Left + 5, headbg.Bottom - 5, headbg.Width - 5,
            headbg.Bottom - 5);

        var space = (headbg.Width - 10) / 120.0;
        var start = Math.round((_heading - 60), 1);

        // draw for outside the 60 deg
        if (_targetheading < start) {
            DrawLine(ctx, "green", 2, headbg.Left + 5 + space * 0, headbg.Bottom,
                headbg.Left + 5 + space * (0), headbg.Top);
        }
        if (_targetheading > _heading + 60) {
            DrawLine(ctx, "green", 2, headbg.Left + 5 + space * 60, headbg.Bottom,
                headbg.Left + 5 + space * (60), headbg.Top);
        }

        for (var a = start; a <= _heading + 60; a += 1) {
            // target heading
            if (((a + 360) % 360) == Math.round(_targetheading)) {
                DrawLine(ctx, "green", 2, headbg.Left + 5 + space * (a - start),
                    headbg.Bottom, headbg.Left + 5 + space * (a - start), headbg.Top);
            }

            if (((a + 360) % 360) == Math.round(_groundcourse)) {
                DrawLine(ctx, "black", 2, headbg.Left + 5 + space * (a - start),
                    headbg.Bottom, headbg.Left + 5 + space * (a - start), headbg.Top);
            }

            if (a % 15 == 0) {
                //Console.WriteLine(a + " " + Math.Round(a, 1, MidpointRounding.AwayFromZero));
                //Console.WriteLine(space +" " + a +" "+ (headbg.Left + 5 + space * (a - start)));
                DrawLine(ctx, "white", 2, headbg.Left + 5 + space * (a - start),
                    headbg.Bottom - 5, headbg.Left + 5 + space * (a - start), headbg.Bottom - 10);
                var disp = a;
                if (disp < 0)
                    disp += 360;
                disp = disp % 360;
                if (disp == 0) {
                    drawstring(ctx, "N", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 45) {
                    drawstring(ctx, "NE", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 90) {
                    drawstring(ctx, "E", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 135) {
                    drawstring(ctx, "SE", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 180) {
                    drawstring(ctx, "S", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 225) {
                    drawstring(ctx, "SW", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 270) {
                    drawstring(ctx, "W", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else if (disp == 315) {
                    drawstring(ctx, "NW", font, fontsize + 4, "white",
                        headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
                else {
                    drawstring(ctx, Math.round(disp % 360, 0), font, fontsize,
                        "white", headbg.Left - 5 + space * (a - start) - fontoffset,
                        headbg.Bottom - 24 - (fontoffset * 1.7));
                }
            }
            else if (a % 5 == 0) {
                DrawLine(ctx, "white", 2, headbg.Left + 5 + space * (a - start),
                    headbg.Bottom - 5, headbg.Left + 5 + space * (a - start), headbg.Bottom - 10);
            }
        }
    }
}
function DrawEllipse(ctx, color, linewidth, x1, y1, width, height) {
    ctx.lineWidth = linewidth;
    ctx.strokeStyle = color;
    ctx.beginPath();
    ctx.moveTo(x1 + width / 2, y1 + height);
    var x, y;
    for (var i = 0; i <= 360; i += 1) {
        x = Math.sin(i * deg2rad) * width / 2;
        y = Math.cos(i * deg2rad) * height / 2;
        x = x + x1 + width / 2;
        y = y + y1 + height / 2;
        ctx.lineTo(x, y);
    }

    //ctx.moveTo(x1,y1);

    ctx.stroke();
    ctx.closePath();
}
function DrawLine(ctx, color, width, x1, y1, x2, y2) {
    ctx.lineWidth = width;
    ctx.strokeStyle = color;
    ctx.beginPath();
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
    ctx.closePath();
}
function DrawPolygon(ctx, color, width, list) {
    ctx.lineWidth = width;
    ctx.strokeStyle = color;
    ctx.beginPath();
    ctx.moveTo(list[0], list[1]);
    for (var i = 2, len = list.length; i < len; i += 2) {
        ctx.lineTo(list[i], list[i + 1]);
    }
    ctx.lineTo(list[0], list[1]);
    ctx.stroke();
    ctx.closePath();
}
function DrawRectangle(ctx, color, headbg) {
    DrawLine(ctx, color, 2, headbg.Left, headbg.Top, headbg.Right, headbg.Top);
    DrawLine(ctx, color, 2, headbg.Right, headbg.Top, headbg.Right, headbg.Bottom);
    DrawLine(ctx, color, 2, headbg.Right, headbg.Bottom, headbg.Left, headbg.Bottom);
    DrawLine(ctx, color, 2, headbg.Left, headbg.Bottom, headbg.Left, headbg.Top);
}
function drawstring(ctx, string, font, fontsize, color, x, y) {
    ctx.font = fontsize + "pt " + font;
    ctx.fillStyle = color;
    ctx.fillText(string, x, y + fontsize);
}