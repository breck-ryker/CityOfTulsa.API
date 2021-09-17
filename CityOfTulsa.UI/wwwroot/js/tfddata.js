


$(document).ready(function () {

	if (!(_commonlib) || !(_commonlib.addListener)) {
		_commonlib = new CommonLib();
	}
	_commonlib.addListener('_ProcessAJAXCallbackResults', ProcessAJAXCallbackResults_TFDData);

   SetCommonHandlers();
});

function ProcessAJAXCallbackResults_TFDData(responseData) {

   if (!(responseData) || !(responseData.parameters) || !(responseData.parameters.cmd)) {
      return;
   }

   switch ((responseData.parameters.cmd || '').toLowerCase()) {

      case 'tfd':

         break;
   }
}

function SetCommonHandlers() {

   $('#show_dateoptions').off('change').on('change', function (e) {
      var $this = $(this);
      var isChecked = $this.prop('checked');
      if (isChecked) {
         $('#dateoptions_btngrp').removeClass('cotHidden');
      }
      else {
         $('#dateoptions_btngrp').addClass('cotHidden');
      }

      CallAJAX('tfd.show-dateoptions', (isChecked ? 1 : 0), null, null, null, true);
   });

   $('#dateoptions_btngrp').off('change').on('change', function (e) {
      var $this = $(this);
      var val = $('#dateoptions_btngrp input:checked').val();

      CallAJAX('tfd.set-dateoption', val, null, null, null, true);
   });
}