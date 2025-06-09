import React, { useEffect, useState } from "react";
import { getTokenFromCookie } from "../../utils";

export default function UserProfile() {
    const [user, setUser] = useState(null);
    const [personInfo, setPersonInfo] = useState(null);
    const [loading, setLoading] = useState(true);

    const [showFavouriteArticles, setShowFavouriteArticles] = useState(false);
    const [showArticlesToRead, setShowArticlesToRead] = useState(false);
    const [showFavouriteTags, setShowFavouriteTags] = useState(false);

    const [favouriteArticles, setFavouriteArticles] = useState([]);
    const [articlesToRead, setArticlesToRead] = useState([]);
    const [favouriteTags, setFavouriteTags] = useState([]);

    const userStateMap = ["Active", "Blocked", "Banned", "Pending"];

    const fetchArticleById = async (id, token) => {
        const res = await fetch(`https://localhost:8083/articles/${id}`, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
        });
        return res.json();
    };

    const fetchTagById = async (id, token) => {
        const res = await fetch(`https://localhost:8083/api/Tags/${id}`, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
        });
        return res.json();
    };

    useEffect(() => {
        const token = getTokenFromCookie();

        fetch("https://localhost:8083/users", {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            }
        })
            .then((res) => res.json())
            .then(async (data) => {
                setUser(data);

                if (data.personInfoId) {
                    fetch("https://localhost:8083/PersonalInfo", {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${token}`,
                        },
                    })
                        .then((res) => res.json())
                        .then((info) => setPersonInfo(info))
                        .catch((err) => console.error("Error fetching personal info:", err));
                }

                // Fetch articles and tags
                const [favArticles, toReadArticles, favTags] = await Promise.all([
                    Promise.all(data.favouriteArticlesIds.map(id => fetchArticleById(id, token))),
                    Promise.all(data.articlesToReadIds.map(id => fetchArticleById(id, token))),
                    Promise.all(data.favouriteTagsIds.map(id => fetchTagById(id, token)))
                ]);

                setFavouriteArticles(favArticles);
                setArticlesToRead(toReadArticles);
                setFavouriteTags(favTags);

                setLoading(false);
            })
            .catch((err) => {
                console.error("Error fetching user:", err);
                setLoading(false);
            });
    }, []);

    if (loading) {
        return <div style={{ padding: "1rem" }}>Loading...</div>;
    }

    if (!user) {
        return <p style={{ color: "red", textAlign: "center" }}>Failed to load user.</p>;
    }

    const Section = ({ title, children }) => (
        <div
            style={{
                border: "1px solid #ddd",
                borderRadius: "8px",
                padding: "1rem",
                marginBottom: "1rem",
                minWidth: "33.33vw", 
                boxSizing: "border-box", 
            }}
        >
            <h3 style={{ marginBottom: "0.5rem" }}>{title}</h3>
            {children}
        </div>
    );


    const Badge = ({ children }) => (
        <span style={{
            display: "inline-block",
            padding: "0.25rem 0.5rem",
            margin: "0.25rem",
            backgroundColor: "#f0f0f0",
            borderRadius: "12px",
            fontSize: "0.85rem"
        }}>{children}</span>
    );

    return (
        <div style={{ maxWidth: "600px", margin: "0 auto", padding: "1rem" }}>
            {personInfo && (
                <Section title="User Info">
                    <p><strong>First Name:</strong> {personInfo.firstName || "N/A"}</p>
                    <p><strong>Last Name:</strong> {personInfo.lastName || "N/A"}</p>
                    <p><strong>Email:</strong> {personInfo.email || "N/A"}</p>
                    <p><strong>Login:</strong> {personInfo.login || "N/A"}</p>
                    <p><strong>State:</strong> {userStateMap[personInfo.state] || "Unknown"}</p>
                    <p><strong>Subscription Active:</strong> {user.subscriptionState ? "Yes" : "No"}</p>
                </Section>
            )}

            <Section title="Favourite Articles">
                <button onClick={() => setShowFavouriteArticles(!showFavouriteArticles)}>
                    {showFavouriteArticles ? "Hide" : "Show"} Favourite Articles
                </button>
                <br/>
                {showFavouriteArticles && favouriteArticles.map((article) => (
                    <Badge key={article.id}>{article.title || `Article ${article.id}`}</Badge>
                ))}
            </Section>

            <Section title="Articles to Read">
                <button onClick={() => setShowArticlesToRead(!showArticlesToRead)}>
                    {showArticlesToRead ? "Hide" : "Show"} Articles to Read
                </button>
                <br />
                {showArticlesToRead && articlesToRead.map((article) => (
                    <Badge key={article.id}>{article.title || `Article ${article.id}`}</Badge>
                ))}
            </Section>

            <Section title="Favourite Tags">
                <button onClick={() => setShowFavouriteTags(!showFavouriteTags)}>
                    {showFavouriteTags ? "Hide" : "Show"} Favourite Tags
                </button>
                <br />
                {showFavouriteTags && favouriteTags.map((tag) => (
                    <Badge key={tag.id}>{tag.name || `Tag ${tag.id}`}</Badge>
                ))}
            </Section>

            <Section title="Comments Activity">
                <p><strong>Posted:</strong> {user.commentsIds.length}</p>
                <p><strong>Liked:</strong> {user.likedCommentsIds.length}</p>
                <p><strong>Disliked:</strong> {user.unlikedCommentsIds.length}</p>
            </Section>
        </div>
    );
}
