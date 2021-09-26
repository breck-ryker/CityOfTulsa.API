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
         'order': [[2, 'asc']]
      });
   
});

function ProcessAJAXCallbackResults_TFDResults(responseData) {

   if (!(responseData) || !(responseData.parameters) || !(responseData.parameters.cmd)) {
      return;
   }

   switch ((responseData.parameters.cmd || '').toLowerCase()) {

      case 'tfd-results.some-command-tbd':

         break;
   }

}