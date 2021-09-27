var _dt = null;

$(document).ready(function () {

   if (!(_commonlib) || !(_commonlib.addListener)) {
      _commonlib = new CommonLib();
   }
   _commonlib.addListener('_ProcessAJAXCallbackResults', ProcessAJAXCallbackResults_TFDResults);

   _dt = $('#search_results')
      .on('init.dt', function () {
         $('#results_container').fadeIn(1000);
      })
      .DataTable({
         'paging': true,
         'ordering': true,
         'lengthMenu': [[10, 15, 20, 25, 50, -1], [10, 15, 20, 25, 50, 'All']],
         'order': [[3, 'asc']],
         'autoWidth': false
      });

   $('#search_results tbody').on('click', 'td.details-control', function () {

      var $this = $(this);
      var incidentid = $this.data('incidentid');
      var $tr = $this.closest('tr');
      var $row = _dt.row($tr);

      if ($row.child.isShown()) {
         // This row is already open - close it
         $row.child.hide();
         $tr.removeClass('shown');
      }
      else {
         // Open this row
         $row.child('<div class="cot-fireevent-detail" data-incidentid="' + incidentid + '"></div>').show();
         $tr.addClass('shown');
         CallAJAX('tfd-results.get-event-vehicles', null, null, [(incidentid || '')], null, false, null);
      }
   });

   $('#search_results tbody').on('click', 'td.map-launcher', function () {

      var $this = $(this);
      var $tr = $this.closest('tr');
      var $row = _dt.row($tr);
      var data = _dt.row(this).data();
      var lat = data[5];
      var lon = data[6];

      var url = 'https://www.google.com/maps/search/?api=1&query=' + encodeURIComponent(lat + ',' + lon);
      window.open(url, '_blank');
   });
   
});

function ProcessAJAXCallbackResults_TFDResults(responseData) {

   if (!(responseData) || !(responseData.parameters) || !(responseData.parameters.cmd)) {
      return;
   }

   switch ((responseData.parameters.cmd || '').toLowerCase()) {

      case 'tfd-results.get-event-vehicles':

         if (responseData.parameters.msg) {
            alertify.dialog('alert').set({ transition: 'zoom' });
            alertify.alert('Run Search', responseData.parameters.msg);
         }

         var returnval = responseData.parameters.returncode;

         if (returnval >= 0 && responseData.parameters.markup && responseData.parameters.ids && responseData.parameters.ids.length == 1) {

            var vehicles = null;

            try {
               vehicles = JSON.parse(responseData.parameters.markup);
            }
            catch (ex) {
               vehicles = null;
            }

            var $details = $('div.cot-fireevent-detail[data-incidentid="' + responseData.parameters.ids[0] + '"]');

            if ($details && $details.length == 1) {

               var outlineMarkup = `
                  <table class="tfd-fireevent-detail ml-5">
                     <thead><tr><th>Division</th><th>Station</th><th>Vehicle</th></tr></thead>
                     <tbody>{body}</tbody>
                  </table>
                  `;

               var bodyMarkup = '';

               function iterateVehicles(item, index, array) {
                  bodyMarkup += '<tr><td>' + (item.division ?? '') + '</td><td>' + (item.station ?? '') + '</td><td>' + (item.vehicleID ?? '') + '</td>';
               }

               vehicles.forEach(iterateVehicles);

               outlineMarkup = outlineMarkup.replace('{body}', bodyMarkup);

               $details.replaceWith(outlineMarkup);
            }
         }

         break;
   }

}