import React from 'react';
import { CheckCircle, ArrowLeft, Mail, Calendar, CreditCard } from 'lucide-react';

const SubscriptionSuccess = () => {
    const styles = {
        container: {
            minHeight: '100vh',
            background: 'linear-gradient(135deg, #f0fdf4 0%, #d1fae5 60%, #dcfce7 90%)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: '16px',
            width: '100vw',
            maxWidth: '100%'
        },
        card: {
            maxWidth: '400px',
            width: '100%',
            backgroundColor: 'white',
            borderRadius: '16px',
            boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.25)',
            padding: '32px',
            textAlign: 'center'
        },
        header: {
            marginBottom: '24px'
        },
        icon: {
            width: '64px',
            height: '64px',
            color: '#22c55e',
            margin: '0 auto 16px'
        },
        title: {
            fontSize: '24px',
            fontWeight: 'bold',
            color: '#111827',
            marginBottom: '8px'
        },
        subtitle: {
            color: '#6b7280'
        },
        infoBox: {
            backgroundColor: '#f0fdf4',
            borderRadius: '8px',
            padding: '16px',
            marginBottom: '24px'
        },
        infoRow: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            fontSize: '14px',
            color: '#166534',
            marginBottom: '8px'
        },
        infoRowLast: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            fontSize: '14px',
            color: '#166534',
            marginBottom: '0'
        },
        infoLabel: {
            display: 'flex',
            alignItems: 'center'
        },
        infoIcon: {
            width: '16px',
            height: '16px',
            marginRight: '8px'
        },
        infoValue: {
            fontWeight: '600'
        },
        buttonContainer: {
            display: 'flex',
            flexDirection: 'column',
            gap: '12px'
        },
        primaryButton: {
            width: '100%',
            backgroundColor: '#16a34a',
            color: 'white',
            fontWeight: '600',
            padding: '12px 24px',
            borderRadius: '8px',
            border: 'none',
            cursor: 'pointer',
            transition: 'background-color 0.2s',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        },
        primaryButtonHover: {
            backgroundColor: '#15803d'
        },
        secondaryButton: {
            width: '100%',
            backgroundColor: '#f3f4f6',
            color: '#374151',
            fontWeight: '600',
            padding: '12px 24px',
            borderRadius: '8px',
            border: 'none',
            cursor: 'pointer',
            transition: 'background-color 0.2s'
        },
        secondaryButtonHover: {
            backgroundColor: '#e5e7eb'
        },
        buttonIcon: {
            width: '16px',
            height: '16px',
            marginLeft: '8px'
        },
        footer: {
            marginTop: '24px',
            paddingTop: '24px',
            borderTop: '1px solid #e5e7eb'
        },
        footerText: {
            fontSize: '12px',
            color: '#6b7280'
        },
        footerLink: {
            color: '#16a34a',
            textDecoration: 'none'
        }
    };

    return (
        <div style={styles.container}>
            <div style={styles.card}>
                <div style={styles.header}>
                    <CheckCircle style={styles.icon} />
                    <h1 style={styles.title}>
                        Subscription Activated!
                    </h1>
                    <p style={styles.subtitle}>
                        Welcome to your premium membership. Your subscription is now active.
                    </p>
                </div>

                <div style={styles.infoBox}>
                    <div style={styles.infoRow}>
                        <span style={styles.infoLabel}>
                            <Calendar style={styles.infoIcon} />
                                Subscription duration 
                        </span>
                        <span style={styles.infoValue}>6 months</span>
                    </div>
                    <div style={styles.infoRowLast}>
                        <span style={styles.infoLabel}>
                            <CreditCard style={styles.infoIcon} />
                            Cost
                        </span>
                        <span style={styles.infoValue}>50 pln</span>
                    </div>
                </div>

                <div style={styles.buttonContainer}>
                    <button
                        style={styles.primaryButton}
                        onClick={() => window.location.href = '/'}
                        onMouseOver={e => e.target.style.backgroundColor = styles.primaryButtonHover.backgroundColor}
                        onMouseOut={e => e.target.style.backgroundColor = styles.primaryButton.backgroundColor}
                    >
                        <ArrowLeft style={styles.buttonIcon} />
                        Back to Home
                    </button>


                </div>

                <div style={styles.footer}>
                    <p style={styles.footerText}>
                        Need help? <a href="/support" style={styles.footerLink}>Contact Support</a>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default SubscriptionSuccess;