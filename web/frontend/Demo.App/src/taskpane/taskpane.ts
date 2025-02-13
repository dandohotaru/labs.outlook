/* global Office, document */

Office.onReady((info) => {
  if (info.host === Office.HostType.Outlook) {
    document.getElementById("sideload-msg")!.style.display = "none";
    document.getElementById("app-body")!.style.display = "flex";
    document.getElementById("summarize-button")!.onclick = summarizeEmail;
  }
});

/**
 * Fetches email summary from backend and prepopulates the response.
 */
async function summarizeEmail() {
  try {
    if (!Office.context.mailbox || !Office.context.mailbox.item) {
      console.error("Not inside Outlook");
      return;
    }

    const item = Office.context.mailbox.item;

    // Get the email ID
    const messageId: string = item.itemId;

    // Call the backend to generate a summary
    const response = await fetch("https://localhost:5001/api/manual-graph/summary", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ messageId })
    });

    if (!response.ok) {
      throw new Error("Failed to fetch summary");
    }

    const data = await response.json();
    const summary: string = data.summary;

    // Open reply window and insert the summary
    item.displayReplyForm({ htmlBody: `<p>${summary}</p>` });

  } catch (error) {
    console.error("Error fetching summary:", error);
  }
}
