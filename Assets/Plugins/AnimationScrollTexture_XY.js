#pragma strict

var Speed_X = 0.0;
var Speed_Y = 0.0;
function FixedUpdate()
{
var offset_x = Time.time * (-Speed_X);
var offset_y = Time.time * (-Speed_Y);
renderer.material.mainTextureOffset = Vector2 (offset_x,offset_y);
}
