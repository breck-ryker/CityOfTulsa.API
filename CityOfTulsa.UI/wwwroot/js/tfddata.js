


$(document).ready(function () {

	if (!(_commonlib) || !(_commonlib.addListener)) {
		_commonlib = new CommonLib();
	}
	_commonlib.addListener('_ProcessAJAXCallbackResults', ProcessAJAXCallbackResults_TFDData);

   SetCommonHandlers();

   CallAJAX('tfd.do-init-validation', null, null, null, null, true);
});

function ProcessAJAXCallbackResults_TFDData(responseData) {

   if (!(responseData) || !(responseData.parameters) || !(responseData.parameters.cmd)) {
      return;
   }

   switch ((responseData.parameters.cmd || '').toLowerCase()) {

      case 'tfd.do-init-validation':

         CheckForInvalidDateMessage(responseData, 1);

         break;

      case 'tfd.date-changed':
      case 'tfd.set-datefilter-option':

         CheckForInvalidDateMessage(responseData, 0);

         break;

      case 'tfd.run-search':

         var returnval = responseData.parameters.returncode;

         if (returnval > 0) {
            $('#show_results').text(' ' + returnval + ' Results');
            $('.cot-results-btn').removeClass('cot-hidden');
         }
         else {
            $('.cot-results-btn').addClass('cot-hidden');
         }

         break;
   }
}

function CheckForInvalidDateMessage(responseData, strictness) {

   var $divInvalidMsg = $('#rowDates + div');

   if ($divInvalidMsg && $divInvalidMsg.length == 1 && $divInvalidMsg.hasClass('cot-validator')) {

      if (responseData.parameters.returncode < 0 && responseData.parameters.msg && (strictness > 0 || responseData.parameters.msgmode != 'no-user-entry')) {
         $divInvalidMsg.find('.cot-validator-msg').text(responseData.parameters.msg);
         $divInvalidMsg.removeClass('cot-hidden');
      }
      else {
         $divInvalidMsg.find('.cot-validator-msg').text('');
         $divInvalidMsg.addClass('cot-hidden');
      }
   }
}

function SetCommonHandlers() {

   $('#show_dateoptions').off('change').on('change', function (e) {

      var $this = $(this);
      var isChecked = $this.prop('checked');

      if (isChecked) {
         $('#dateoptions_btngrp').removeClass('cot-hidden');
      }
      else {
         $('#dateoptions_btngrp').addClass('cot-hidden');
         $('#rowDates + div.cot-validator').addClass('cot-hidden');
      }

      if (!(isChecked)) {
         SetDatepickersVisibility(0);
      }
      else {
         var dateoption = $('#dateoptions_btngrp input:checked').val();
         SetDatepickersVisibility(dateoption);
      }

      CallAJAX('tfd.show-datefilter-options', (isChecked ? 1 : 0), null, null, null, true);
   });

   $('#dateoptions_btngrp').off('change').on('change', function (e) {

      var $this = $(this);
      var dateoption = $('#dateoptions_btngrp input:checked').val();

      SetDatepickersVisibility(dateoption);

      CallAJAX('tfd.set-datefilter-option', dateoption, null, null, null, true);
   });

   SetDatepickers();

   // activating / deactivating the overall group of options
   $('input.cot-option-chkbox').off('change').on('change', function (e) {

      var $this = $(this);
      var isChecked = $this.prop('checked');
      var btngrpid = $this.data('btngrpid');
      var listrowid = $this.data('listrowid');
      var cmd = $this.data('cotcmd');

      if (isChecked) {
         $('#' + listrowid).removeClass('cot-hidden');
         $('#' + btngrpid).removeClass('cot-hidden');
      }
      else {
         $('#' + listrowid).addClass('cot-hidden');
         $('#' + btngrpid).addClass('cot-hidden');
      }

      CallAJAX(cmd, (isChecked ? 1 : 0), null, null, null, true);
   });

   // checking / unchecking a single item in the list
   $('input.cot-chkbox-item').off('change').on('change', function (e) {

      var $this = $(this);
      var isInUse = $this.data('is-in-use');
      if (isInUse > 0) {
         return;
      }
      var isChecked = $this.prop('checked');
      var $parent = $this.closest('div');
      var $grpparent = $parent.closest('ul.cot-chkbox-list');
      var cmd = $grpparent.data('cotcmd');
      var $label = $parent.find('label');
      var lbl = $label.text();

      CallAJAX(cmd, (isChecked ? 1 : 0), lbl, null, null, true);
   });

   // clicking a select all / unselect all button
   $('.cot-selectall-btngrp button').off('click').on('click', function (e) {

      var $this = $(this);
      var action = $this.data('action');
      var $parent, $label, lbl, actiontarget, cmd;
      var ary = [];

      $parent = $this.parent();
      actiontarget = $parent.data('actiontarget');
      cmd = $parent.data('cotcmd');

      $('input.' + actiontarget).each(function (i, elm) {
         var $this = $(elm);
         $this.data('is-in-use', 1);
         $this.prop('checked', (action == 'select-all' ? true : false));
         $parent = $this.closest('div');
         $label = $parent.find('label');
         lbl = $label.text();
         ary.push(lbl);
         $this.data('is-in-use', 0);
      });

      CallAJAX(cmd, null, action, null, ary, true);
   });
}

function SetDatepickers() {

   var dtToday = new Date();
   var sToday = (dtToday.getMonth() + 1) + '/' + dtToday.getDate() + '/' + dtToday.getFullYear();

   $('input.datepicker').datepicker({
      autoclose: true,
      format: 'mm/dd/yyyy',
      startDate: '09/14/2021',
      endDate: sToday,
      todayHighlight: true
   }).on('changeDate', function (e) {
      var $this = $(this);
      var id = $this.attr('id');
      var dt = e.date;
      var data = (dt.getMonth() + 1) + '/' + dt.getDate() + '/' + dt.getFullYear();
      CallAJAX('tfd.date-changed', data, id, [id], null, true);
   });
}

function SetDatepickersVisibility(dateoption) {

   if (dateoption == 4) {
      $('#mindate').attr('placeholder', 'min date');
      $('#maxdate_container,#maxdate_label').removeClass('cot-hidden');
   }
   else {
      $('#mindate').attr('placeholder', 'select date');
      $('#maxdate_container,#maxdate_label').addClass('cot-hidden');
   }

   if (dateoption > 0) {

      $('#rowDates').removeClass('cot-hidden');

      SetDatepickers();
   }
   else {
      $('#rowDates').addClass('cot-hidden');
   }
}