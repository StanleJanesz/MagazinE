import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "motion/react";
import ArticleTile from "../../Components/ListTile/ArticleTile/ArticleTile";
import "./AdminPage.css";

function AdminPage() {
    const [isLoading, setIsLoading] = useState(true);
    const [reports, setReports] = useState([]);
    const [unbanRequests, setUnbanRequests] = useState([]);
    const [view, setView] = useState("reports");
    const [chosenItemId, setChosenItemId] = useState(Number.MAX_SAFE_INTEGER);
    const [chosenItem, setChosenItem] = useState('');

    const unbanRequestStates = Object.freeze({
        pending: 'Pending',
        denied: 'Denied',
        accepted: 'Accepted'
    });

    const unbanData = [
        { id: 0, reason: "I have never seen this comment in my life!", banId: 0, unbanRequestState: unbanRequestStates.pending },
        { id: 1, reason: "Please unblock me! My cat stepped on the keyboard!", banId: 1, unbanRequestState: unbanRequestStates.pending },
        { id: 2, reason: "Autocorrect betrayed me.", banId: 2, unbanRequestState: unbanRequestStates.pending },
        { id: 3, reason: "My cat stepped on the keyboard!", banId: 3, unbanRequestState: unbanRequestStates.pending },
        { id: 4, reason: "I was doing a social experiment.", banId: 4, unbanRequestState: unbanRequestStates.pending }
    ];

    const reportData = [
        { id: 0, commentId: 101, reportAuthorId: 201, reason: "Tried to start a cult in the comment section.", date: new Date('2025-04-01T14:32:00'), result: false },
        { id: 1, commentId: 102, reportAuthorId: 202, reason: "Posted spoilers to a movie that doesn't even exist yet.", date: new Date('2025-04-02T10:15:00'), result: false },
        { id: 2, commentId: 103, reportAuthorId: 203, reason: "Commented only using Morse code.", date: new Date('2025-04-03T09:00:00'), result: false },
        { id: 3, commentId: 104, reportAuthorId: 204, reason: "Challenged the admin to a duel in every post.", date: new Date('2025-04-04T20:45:00'), result: false },
        { id: 4, commentId: 105, reportAuthorId: 205, reason: "Started every comment with 'As a time traveler...'", date: new Date('2025-04-05T17:25:00'), result: false },
        { id: 5, commentId: 106, reportAuthorId: 206, reason: "Used 48 emojis in a single comment. No words.", date: new Date('2025-04-06T08:10:00'), result: false }
    ];

    // mimic API
    const fetchData = async () => {
        setIsLoading(true);
        setReports(reportData);
        setUnbanRequests(unbanData);
        setIsLoading(false);
    };

    useEffect(() => {
        fetchData();
    }, []);

    const itemsToShow = view === "reports" ? reports : unbanRequests;

    const handleRemove = async (id) => {
        if (view === "reports") {
            setReports(reports.filter(report => report.id !== id));
        } else {
            setUnbanRequests(unbanRequests.filter(unban => unban.id !== id));
        }
    }
   
    const contents = isLoading ? (
        <h1>Loading…</h1>
    ) : (
        <AnimatePresence mode="popLayout">
            {itemsToShow.map(item => (
                <motion.div
                    key={item.id}
                    layout
                    layoutTransition={{ type: "spring", stiffness: 900, damping: 40 }}
                    initial={{ opacity: 0, x: -50 }}
                    animate={{ opacity: 1, x: 0 }}
                    exit={{ opacity: 0, scale: 0 }}
                >
                    <ArticleTile
                        id={item.id}
                        title={item.reason}
                        className={view === "reports" ? "reports-title" : "requests-title"}
                        reject={() => handleRemove(item.id)}
                        accept={() => handleRemove(item.id)}
                        onSelect={(id) => {
                            setChosenItemId(id);
                            setChosenItem(
                                view === "reports" ?
                                    reports.find(report => report.id === id) :
                                    unbanRequests.find(request => request.id === id));
                        }}
                        isChosen={chosenItemId === item.id}
                        photo="src/assets/user.png"
                    />
                </motion.div>
            ))}
        </AnimatePresence>
    );

    const content =
        chosenItemId === Number.MAX_SAFE_INTEGER || !chosenItem ? (
            <h2 style={{ fontStyle: "italic", color: "#777" }}>Request preview</h2>
        ) : (
            <div style={{ backgroundColor: "#fff", padding: "20px", borderRadius: "8px", boxShadow: "0 2px 10px rgba(0,0,0,0.1)" }}>
                <p><strong>From:</strong> User #{chosenItem.reportAuthorId ?? chosenItem.banId}</p>
                {chosenItem.date && (
                    <p><strong>Date:</strong> {new Date(chosenItem.date).toLocaleString()}</p>
                )}
                <hr style={{ margin: "12px 0" }} />
                <p style={{ whiteSpace: "pre-wrap" }}>{chosenItem.reason}</p>
            </div>
        );

    return (
        <div className="ajp-container">
            <div className="ajp-articlesView">
                {/* Tab Navigation */}
                <div className="tabs" style={{ display: "flex", position: 'relative', marginBottom: '0' }}>
                    <motion.button
                        className={`tab ${view === "reports" ? "active" : ""}`}
                        onClick={() => setView("reports")}
                        style={{
                            borderTop: view === "reports" ? "4px solid #939F5C" : "4px solid transparent",
                            borderLeft: view === "reports" ? "4px solid #939F5C" : "4px solid transparent",
                            borderRight: view === "reports" ? "4px solid #939F5C" : "4px solid transparent",
                            borderBottom: 'transparent',
                            backgroundColor: view === "reports" ? "#939F5C" : "transparent",
                            borderRadius: '8px 8px 0 0',
                            marginBottom: '0',
                        }}
                        layoutId={view}
                    >
                        Reports
                    </motion.button>
                    <motion.button
                        className={`tab ${view === "requests" ? "active" : ""}`}
                        onClick={() => setView("requests")}
                        style={{
                            borderTop: view === "requests" ? "4px solid #939F5C" : "4px solid transparent",
                            borderLeft: view === "requests" ? "4px solid #939F5C" : "4px solid transparent",
                            borderRight: view === "requests" ? "4px solid #939F5C" : "4px solid transparent",
                            borderBottom: 'transparent',
                            backgroundColor: view === "requests" ? "#939F5C" : "transparent",
                            borderRadius: '8px 8px 0 0',
                            marginBottom: '0',

                        }}
                    >
                        Unban Requests
                    </motion.button>
                </div>
                <div
                    style={{
                        marginTop: '0',
                        border: '4px solid #939F5C',
                        borderRadius: view === "reports" ? '0px 8px 8px 8px' : '8px',
                        padding: '15px',
                        backgroundColor: "#939F5C"
                    }}
                >
                    {contents}
                </div>
             
            </div>


            <div className="ajp-articlePreview">
                {content}
            </div>
        </div>
    );
}

export default AdminPage;
