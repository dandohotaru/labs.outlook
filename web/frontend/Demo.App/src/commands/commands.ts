/* global Office */

Office.onReady(() => {
  // Ensure Office.js is ready before running
});

/**
 * Opens the task pane when the add-in button is clicked.
 */
function showTaskpane(event: Office.AddinCommands.Event) {
  Office.context.ui.displayDialogAsync("taskpane.html", { height: 50, width: 50 });
  event.completed();
}

// Register the action with Office.js
Office.actions.associate("showTaskpane", showTaskpane);
