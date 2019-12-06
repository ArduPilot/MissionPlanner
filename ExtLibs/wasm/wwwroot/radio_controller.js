// renders a radio controller using Two.js (https://two.js.org)
if(radio_controller === undefined) {

var radio_controller = {
	_two: undefined,
	_left_stick: undefined,
	_right_stick: undefined,
	_flight_mode_text: undefined,
	_stick_history: undefined,

	init: function(elem, width, height) {
		var params = { width: width, height: height };
		var two = new Two(params).appendTo(elem);
		this._two = two;

		/* render the radio */
		var frame_color = '#999999';
		var line_width = 3;

		// outer frame
		var rect = two.makeRoundedRectangle(width/2, height/2, width-line_width,
			height-line_width, 30);
		rect.stroke = frame_color;
		rect.linewidth = line_width;
		rect.fill = 'rgba(230, 230, 220, 0.6)';

		this._left_stick = this._drawStick(two, width / 4, height / 2 * 0.8,
			width / 4 * 0.8, frame_color, line_width);
		this._right_stick = this._drawStick(two, width * 3 / 4, height / 2 * 0.8,
			width / 4 * 0.8, frame_color, line_width);

		var flight_mode = new Two.Text("", width/2, height*0.85);
		flight_mode.family = 'monospace';
		flight_mode.size = 16;
		two.add(flight_mode);
		this._flight_mode_text = flight_mode;

		// Don't forget to tell two to render everything to the screen
		two.update();
	},

	updateHistory: function(values) {
		// draw a fading line from past values.
		// values: list where each element is
		// a list of 4 values: [x-left, y-left, x-right, y-right].
		// first item is the oldest

		two = this._two;

		// remove existing history
		// Note: not very efficient, just the simplest thing to do
		if (this._stick_history !== undefined) {
			for (var i = 0; i < this._stick_history.length; ++i) {
				two.remove(this._stick_history[i]);
			}
			this._stick_history = undefined;
		}

		if (values === undefined || values.length < 2)
			return;

		var history = [];
		var rects = [this._left_stick.rect, this._right_stick.rect];
		for (var stick_idx = 0; stick_idx < 2; ++stick_idx) { // left & right
			var x_pos_last = rects[stick_idx].x + (1+values[0][0+stick_idx*2])
				* rects[stick_idx].w/2;
			var y_pos_last = rects[stick_idx].y + (1-values[0][1+stick_idx*2])
				* rects[stick_idx].h/2;
			for (var i = 1; i < values.length; ++i) {
				var x_pos = rects[stick_idx].x + (1+values[i][0+stick_idx*2]) *
					rects[stick_idx].w/2;
				var y_pos = rects[stick_idx].y + (1-values[i][1+stick_idx*2]) *
					rects[stick_idx].h/2;
				var line = two.makeLine(x_pos_last, y_pos_last, x_pos, y_pos);
				line.linewidth = 2;
				var alpha = 0.1 + 0.8 * (i-1) / (values.length-2);
				line.stroke = 'rgba(255, 128, 0, '+alpha.toString()+')';
				x_pos_last = x_pos;
				y_pos_last = y_pos;
				history.push(line);
			}
		}

		this._stick_history = history;
	},

	_drawStick: function(two, cx, cy, radius, frame_color, line_width) {
		var circle = two.makeCircle(cx, cy, radius); // outer circle
		circle.stroke = frame_color;
		circle.linewidth = line_width;
		circle.noFill();

		var radius_inner = radius * 0.85;
		var circle = two.makeCircle(cx, cy, radius_inner); // inner circle
		circle.stroke = frame_color;
		circle.linewidth = line_width;
		circle.noFill();

		var inner_color = '#666666';
		var angle = 45 * Math.PI / 180; // 45 == square
		var rect_width = 2 * (radius_inner - line_width) * Math.cos(angle);
		var rect_height = 2 * (radius_inner - line_width) * Math.sin(angle);
		var rect = two.makeRectangle(cx, cy, rect_width, rect_height); // inner rect
		var rect_dim = { x: cx-rect_width/2, y: cy-rect_height/2,
			w: rect_width, h: rect_height };
		rect.stroke = inner_color;
		rect.linewidth = line_width/2;
		rect.noFill();

		// markers
		var marker_size = 5;
		var marker_line_width = 1;
		var x = rect_dim.x, y = rect_dim.y, w = rect_dim.w, h = rect_dim.h;
		var line = two.makeLine(x, y+h/2, x+marker_size, y+h/2);
		line.linewidth = marker_line_width;
		line.stroke = inner_color;

		line = two.makeLine(x+w, y+h/2, x+w-marker_size, y+h/2);
		line.linewidth = marker_line_width;
		line.stroke = inner_color;

		line = two.makeLine(x+w/2, y, x+w/2, y+marker_size);
		line.linewidth = marker_line_width;
		line.stroke = inner_color;

		line = two.makeLine(x+w/2, y+h, x+w/2, y+h-marker_size);
		line.linewidth = marker_line_width;
		line.stroke = inner_color;

		// center marker
		var center_marker_size = 16;
		line = two.makeLine(x+w/2-center_marker_size/2, y+h/2, x+w/2+center_marker_size/2, y+h/2);
		line.linewidth = marker_line_width;
		line.stroke = inner_color;
		line = two.makeLine(x+w/2, y+h/2-center_marker_size/2, x+w/2, y+h/2+center_marker_size/2);
		line.linewidth = marker_line_width;
		line.stroke = inner_color;

		// stick position
		var stick = two.makeCircle(cx, cy, 5);
		stick.fill = '#FF8000';
		stick.noStroke();

		return {rect: rect_dim, stick: stick};
	},

	_moveStick: function(two, stick_data, x, y) {
		// x, y in range [-1, 1]
		var stick = stick_data.stick;
		stick.opacity = 1;
		var rect = stick_data.rect;
		var x_pos = rect.x + (1+x) * rect.w/2;
		var y_pos = rect.y + (1-y) * rect.h/2;
		stick.translation.set(x_pos, y_pos);
	},

	updateSticks: function(x_left, y_left, x_right, y_right) {
		// update stick positions (all value are in range [-1, 1])
		this._moveStick(this._two, this._left_stick, x_left, y_left);
		this._moveStick(this._two, this._right_stick, x_right, y_right);
	},

	hideSticks: function() {
		this._left_stick.stick.opacity = 0;
		this._right_stick.stick.opacity = 0;
	},

	setFlightMode: function(flight_mode) {
		this._flight_mode_text.value = flight_mode;
	},

	redraw: function() {
		this._two.update();
	},
};

}