#include <Adafruit_NeoPixel.h>

#define PIN 0
#define LENGTH 10
#define ON 40
#define DELAY 75

// Parameter 1 = number of pixels in strip
// Parameter 2 = Arduino pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
//   NEO_RGBW    Pixels are wired for RGBW bitstream (NeoPixel RGBW products)
Adafruit_NeoPixel strip1 = Adafruit_NeoPixel(LENGTH + 1, PIN, NEO_RGB + NEO_KHZ800  );
Adafruit_NeoPixel strip2 = Adafruit_NeoPixel(LENGTH + 1, PIN+1, NEO_RGB + NEO_KHZ800  );
Adafruit_NeoPixel strip3 = Adafruit_NeoPixel(LENGTH + 1, PIN+2, NEO_RGB + NEO_KHZ800  );
Adafruit_NeoPixel strip4 = Adafruit_NeoPixel(LENGTH + 1, PIN+3, NEO_RGB + NEO_KHZ800  );

void setup() {
  // put your setup code here, to run once:

  strip1.begin();
  strip1.show(); // Initialize all pixels to 'off'

  strip2.begin();
  strip2.show(); // Initialize all pixels to 'off'

  strip3.begin();
  strip3.show(); // Initialize all pixels to 'off'

  strip4.begin();
  strip4.show(); // Initialize all pixels to 'off'
}

void loop() {
  // put your main code here, to run repeatedly:
  colorWipe(strip1.Color(ON, 0, 0), DELAY); // Red
  colorWipe(strip1.Color(0, ON, 0), DELAY); // Green
  colorWipe(strip1.Color(0, 0, ON), DELAY); // Blue
}

// Fill the dots one after the other with a color
void colorWipe(uint32_t c, uint8_t wait) {
  for(uint16_t i=0; i<strip1.numPixels(); i++) {
    if(i==strip1.numPixels()-1)
    {
      // hack for using a different neopixel as the tester
      uint8_t r = (0xFF0000 & c) >> 16;
      uint8_t g = (0xFF00 & c) >> 8;
      uint8_t b = 0xFF & c;
      c = strip1.Color(g,r,b);
    }
    
    strip1.setPixelColor(i, c);
    strip1.show();
    
    strip2.setPixelColor(i, c);
    strip2.show();
    
    strip3.setPixelColor(i, c);
    strip3.show();
    
    strip4.setPixelColor(i, c);
    strip4.show();
    delay(wait);
  }
}
