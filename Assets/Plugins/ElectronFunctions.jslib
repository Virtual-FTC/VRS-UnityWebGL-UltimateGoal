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
});
