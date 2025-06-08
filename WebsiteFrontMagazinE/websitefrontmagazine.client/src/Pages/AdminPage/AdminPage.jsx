import React, { useEffect, useState } from "react";

export default function AdminPage() {
    const [reports, setReports] = useState([]);
    const [unbanRequests, setUnbanRequests] = useState([]);

    const fetchReports = async () => {
        try {
            const token = localStorage.getItem("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const response = await fetch("https://localhost:7054/reports/pending", {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                throw new Error("Failed to fetch pending reports");
            }

            const reportIds = await response.json();

            // Pobieramy pe³ne dane dla ka¿dego ID
            const reportsData = await Promise.all(
                reportIds.map(async (id) => {
                    const res = await fetch(`https://localhost:7054/reports/report/${id}`, {
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

            // Filtrujemy null (nieudane fetch)
            setReports(reportsData.filter((r) => r !== null));
        } catch (error) {
            console.error("Error fetching reports:", error);
        }
    };

    const fetchUnbanRequests = () => {
        // Tutaj masz mockowane dane, zostawiam tak jak jest
        setUnbanRequests([
            { id: 1, login: "user1", reason: "Cheating" },
            { id: 2, login: "user2", reason: "Inappropriate language" },
        ]);
    };

    const handleResolveReport = async (id, newState) => {
        try {
            const token = localStorage.getItem("jwt");
            if (!token) {
                console.error("No JWT token found");
                return;
            }

            const response = await fetch("https://localhost:7054/reports", {
                method: "PUT",
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ id: id, state: newState }),
            });

            if (!response.ok) {
                throw new Error("Failed to resolve report");
            }

            // Odœwie¿amy listê po zmianie statusu
            fetchReports();
        } catch (error) {
            console.error("Error resolving report:", error);
        }
    };

    useEffect(() => {
        fetchReports();
        fetchUnbanRequests();
    }, []);

    return (
        <>
            <h1>Reports:</h1>
            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Reason</th>
                        <th>State</th>
                        <th>Actions</th>
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
                                <button onClick={() => handleResolveReport(report.id, "Accepted")}>
                                    Accept
                                </button>
                                <button onClick={() => handleResolveReport(report.id, "Rejected")}>
                                    Reject
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <h1>Unban Requests:</h1>
            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Login</th>
                        <th>Reason</th>
                    </tr>
                </thead>
                <tbody>
                    {unbanRequests.length === 0 && (
                        <tr>
                            <td colSpan="3">No unban requests found</td>
                        </tr>
                    )}
                    {unbanRequests.map((request) => (
                        <tr key={request.id}>
                            <td>{request.id}</td>
                            <td>{request.login}</td>
                            <td>{request.reason}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}

