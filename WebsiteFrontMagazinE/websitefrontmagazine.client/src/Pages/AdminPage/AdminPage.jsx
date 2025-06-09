import React, { useEffect, useState } from "react";
import './AdminPage.css';

export default function AdminPage() {
    const [reports, setReports] = useState([]);
    const [unbanRequests, setUnbanRequests] = useState([]);
    const [view, setView] = useState("reports");
    const [selectedComment, setSelectedComment] = useState(null);
    const [showModal, setShowModal] = useState(false);

    function getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(";").shift();
        return null;
    }

    const fetchCommentById = async (commentId) => {
        try {
            const token = getCookie("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const response = await fetch(`https://localhost:8083/api/comments/${commentId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Failed to fetch comment: ${errorText}`);
            }

            const data = await response.json();


            // Extract user ID from comment (deep nested)
            const userId = data.authorId;
            if (!userId) {
                throw new Error("User ID not found in comment");
            }

            // Fetch user info from /PersonalInfo/{userId}
            const userInfoRes = await fetch(`https://localhost:8083/PersonalInfo/${userId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    Accept: "text/plain"
                }
            });

            if (!userInfoRes.ok) {
                throw new Error("Failed to fetch user info");
            }

            const userInfo = await userInfoRes.json();

            // Combine comment + user data
            setSelectedComment({
                ...data,
                authorInfo: userInfo
            });


            setShowModal(true); // otwieramy modal
        } catch (error) {
            console.error("Error fetching comment:", error);
        }
    };


    const fetchReports = async () => {
        try {
            const token = getCookie("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const response = await fetch("https://localhost:8083/reports/pending", {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                throw new Error("Failed to fetch pending reports");
            }

            const reportIds = await response.json();

            const reportsData = await Promise.all(
                reportIds.map(async (id) => {
                    const res = await fetch(`https://localhost:8083/reports/report/${id}`, {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    });
                    if (!res.ok) {
                        console.error(`Failed to fetch report ${id}`);
                        return null;
                    }
                    return res.json();
                })
            );

            setReports(reportsData.filter((r) => r !== null));
        } catch (error) {
            console.error("Error fetching reports:", error);
        }
    };

    const fetchUnbanRequests = async () => {
        try {
            const token = getCookie("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const response = await fetch("https://localhost:8083/unbanRequests/pending", {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                throw new Error("Failed to fetch unban requests");
            }

            const data = await response.json();
            setUnbanRequests(data);
        } catch (error) {
            console.error("Error fetching unban requests:", error);
        }
    };

    const handleResolveReport = async (report, newState) => {
        try {
            const token = getCookie("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const params = {
                ManagedById: report.managedById,
                Id: report.id,
                CommentId: report.commentId,
                ReportAuthorId: report.reportAuthorId,
                Reason: report.reason,
                Date: report.date,
                State: newState
            };

            const queryString = new URLSearchParams(params).toString();

            const response = await fetch(`https://localhost:8083/reports?${queryString}`, {
                method: "PUT",
                headers: {
                    Authorization: `Bearer ${token}`,
                    Accept: "*/*"
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Failed to resolve report: ${errorText}`);
            }

            fetchReports();
        } catch (error) {
            console.error("Error resolving report:", error);
        }
    };


    const handleResolveUnbanRequest = async (request, newState) => {
        try {
            const token = getCookie("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const solvedById = 1;
            const query = new URLSearchParams({
                SolvedById: solvedById,
                Id: request.id,
                Reason: request.reason,
                BanId: request.banId,
                State: newState
            }).toString();

            const response = await fetch(`https://localhost:8083/unbanRequests/${request.id}?${query}`, {
                method: "PUT",
                headers: {
                    Authorization: `Bearer ${token}`,
                    Accept: "*/*"
                }
            });

            if (!response.ok) {
                
                const errorText = await response.text();
                throw new Error(`Failed to resolve unban request: ${errorText}`);
            }

            fetchUnbanRequests();
        } catch (error) {
            console.error("Error resolving unban request:", error);
        }
    };


    useEffect(() => {
        fetchReports();
        fetchUnbanRequests();
    }, []);

    return (
        <>
            
            <div className="view-picker">
                <label htmlFor="view-select">Select View:</label>
                <select
                    id="view-select"
                    value={view}
                    onChange={(e) => setView(e.target.value)}
                >
                    <option value="reports">Reports</option>
                    <option value="unban">Unban Requests</option>
                </select>
            </div>

            {view === "reports" && (
                <>
                    <h1 className="reports-title">Reports:</h1>
                    <table>
                        <thead>
                            <tr>
                                <th>Id</th>
                                <th>Reason</th>
                                <th>State</th>
                                <th>Actions</th>
                                <th>Comment</th>
                            </tr>
                        </thead>
                        <tbody>
                            {reports.length === 0 && (
                                <tr>
                                    <td colSpan="4">No reports found</td>
                                </tr>
                            )}
                            {reports.map((report) => (
                                <tr key={report.id}>
                                    <td>{report.id}</td>
                                    <td>{report.reason}</td>
                                    <td>{report.state}</td>
                                    <td>
                                        <button onClick={() => handleResolveReport(report, 1)}>
                                            Accept
                                        </button>
                                        <button onClick={() => handleResolveReport(report, 2)}>
                                            Reject
                                        </button>
                                    </td>
                                    <td>
                                        <button onClick={() => fetchCommentById(report.commentId)}>
                                            See Comment
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}

            {showModal && selectedComment && (
                <div className="modal-backdrop" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <h2>Comment Preview</h2>
                        <p><strong>Content:</strong> {selectedComment.content}</p>
                        <h4>Author:</h4>
                        <p>{selectedComment.authorInfo.firstName} {selectedComment.authorInfo.lastName}</p>
                        <p>Email: {selectedComment.authorInfo.email}</p>
                        <button onClick={() => setShowModal(false)}>Close</button>
                    </div>
                </div>
            )}

           


            {view === "unban" && (
                <>
                    <h1 className="requests-title">Unban Requests:</h1>
                    <table>
                        <thead>
                            <tr>
                                <th>Id</th>
                                <th>BanId</th>
                                <th>Reason</th>
                                <th>State</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {unbanRequests.length === 0 && (
                                <tr>
                                    <td colSpan="5">No unban requests found</td>
                                </tr>
                            )}
                            {unbanRequests.map((request) => (
                                <tr key={request.id}>
                                    <td>{request.id}</td>
                                    <td>{request.banId}</td>
                                    <td>{request.reason}</td>
                                    <td>{request.state}</td>
                                    <td>
                                        <button onClick={() => handleResolveUnbanRequest(request, 0)}>
                                            Accept
                                        </button>
                                        <button onClick={() => handleResolveUnbanRequest(request, 1)}>
                                            Reject
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}
        </>
    );
}

