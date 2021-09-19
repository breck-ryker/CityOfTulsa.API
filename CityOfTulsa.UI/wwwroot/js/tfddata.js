


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

      if (!(isChecked)) {
         SetDatepickersVisibility(0);
      }
      else {
         var dateoption = $('#dateoptions_btngrp input:checked').val();
         SetDatepickersVisibility(dateoption);
      }

      CallAJAX('tfd.show-dateoptions', (isChecked ? 1 : 0), null, null, null, true);
   });

   $('#dateoptions_btngrp').off('change').on('change', function (e) {

      var $this = $(this);
      var dateoption = $('#dateoptions_btngrp input:checked').val();

      SetDatepickersVisibility(dateoption);

      CallAJAX('tfd.set-dateoption', dateoption, null, null, null, true);
   });

   SetDatepickers();

   $('#show_problemlist').off('change').on('change', function (e) {

      var $this = $(this);
      var isChecked = $this.prop('checked');

      if (isChecked) {
         $('#rowProblemList').removeClass('cotHidden');
         $('#problemoptions_btngrp').removeClass('cotHidden');
      }
      else {
         $('#rowProblemList').addClass('cotHidden');
         $('#problemoptions_btngrp').addClass('cotHidden');
      }

      CallAJAX('tfd.show-problemlist', (isChecked ? 1 : 0), null, null, null, true);
   });

   $('input.problem-item').off('change').on('change', function (e) {

      var $this = $(this);
      var isInUse = $this.data('is-in-use');
      if (isInUse > 0) {
         return;
      }
      var isChecked = $this.prop('checked');
      var $parent = $this.closest('div');
      var $label = $parent.find('label');
      var lbl = $label.text();

      CallAJAX('tfd.select-problem', (isChecked ? 1 : 0), lbl, null, null, true);
   });

   $('#problemoptions_btngrp button').off('click').on('click', function (e) {

      var $this = $(this);
      var action = $this.data('action');
      var $parent, $label, lbl;
      var ary = [];

      $('input.problem-item').each(function (i, elm) {
         var $this = $(elm);
         $this.data('is-in-use', 1);
         $this.prop('checked', (action == 'select-all' ? true : false));
         $parent = $this.closest('div');
         $label = $parent.find('label');
         lbl = $label.text();
         ary.push(lbl);
         $this.data('is-in-use', 0);
      });

      CallAJAX('tfd.multi-select-problems', null, action, null, ary, true);
   });
}

function SetDatepickers() {

   var dtToday = new Date();
   var sToday = (dtToday.getMonth() + 1) + '/' + dtToday.getDate() + '/' + dtToday.getFullYear();

   $('input.datepicker').datepicker({
      format: 'mm/dd/yyyy',
      startDate: '09/14/2021',
      endDate: sToday,
      todayHighlight: true
   });
}

function SetDatepickersVisibility(dateoption) {

   if (dateoption == 4) {
      $('#mindate').attr('placeholder', 'min date');
      $('#maxdate_container,#maxdate_label').removeClass('cotHidden');
   }
   else {
      $('#mindate').attr('placeholder', 'select date');
      $('#maxdate_container,#maxdate_label').addClass('cotHidden');
   }

   if (dateoption > 0) {

      $('#rowDates').removeClass('cotHidden');

      SetDatepickers();
   }
   else {
      $('#rowDates').addClass('cotHidden');
   }
}