mergeInto(LibraryManager.library, {
  updateFrontLeftEncoders: function (encoders) {
    setFrontLeftEncoders(encoders);
  },
  updateFrontRightEncoders: function (encoders) {
    setFrontRightEncoders(encoders);
  },
  updateBackLeftEncoders: function (encoders) {
    setBackLeftEncoders(encoders);
  },
  updateBackRightEncoders: function (encoders) {
    setBackRightEncoders(encoders);
  },
updateColorSensorData: function (R,G,B,Distance) {
    setColorSensorData(R,G,B,Distance);
  },
updateDistanceSensorData: function (Distance) {
    setDistanceSensorData(Distance);
  },
updateTouchSensorData: function (Touch) {
    setTouchSensorData(Touch);
  },
updateIMUSensorData: function (x,y,z,angularX,angularY,angularZ,positionX,positionY,positionZ) {
    setIMUSensorData(x,y,z,angularX,angularY,angularZ,positionX,positionY,positionZ);
  }
});
