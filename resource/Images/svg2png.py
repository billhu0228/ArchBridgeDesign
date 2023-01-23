from svglib.svglib import svg2rlg
from reportlab.graphics import renderPDF, renderPM

drawing = svg2rlg("Front View-595b40b65ba036ed117d11a5.svg")
renderPM.drawToFile(drawing, "front.png", fmt="PNG")
