var RotateIcon = function (options) {
    this.options = options || {};
    this.rImg = options.img || new Image();
    this.rImg.setAttribute('crossorigin', 'anonymous');
    this.rImg.src = this.rImg.src || this.options.url || '/static/groups/img/car_map_state_go.png';
    this.options.width = this.options.width || this.rImg.width || 52;
    this.options.height = this.options.height || this.rImg.height || 60;
    canvas = document.createElement("canvas");
    canvas.width = this.options.width;
    canvas.height = this.options.height;
    this.context = canvas.getContext("2d");
    this.canvas = canvas;
};
RotateIcon.makeIcon = function (url) {
    return new RotateIcon({ url: url });
};
RotateIcon.prototype.setRotation = function (options) {
    var canvas = this.context,
        angle = options.deg ? options.deg * Math.PI / 180 :
            options.rad,
        centerX = this.options.width / 2,
        centerY = this.options.height / 2;

    canvas.clearRect(0, 0, this.options.width, this.options.height);
    canvas.save();
    canvas.translate(centerX, centerY);
    canvas.rotate(angle);
    canvas.translate(-centerX, -centerY);
    canvas.drawImage(this.rImg, 0, 0);
    canvas.restore();
    return this;
};
RotateIcon.prototype.getUrl = function () {
    return this.canvas.toDataURL('image/png');
};
