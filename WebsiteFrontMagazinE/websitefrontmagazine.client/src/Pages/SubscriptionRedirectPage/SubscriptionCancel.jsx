import React from 'react';
import { useNavigate } from 'react-router-dom';
import { XCircle, ArrowLeft, RefreshCw, MessageCircle } from 'lucide-react';
import { startCheckout } from '../../utils';

const SubscriptionCancel = () => {
    const navigate = useNavigate();
    

    const styles = {
        container: {
            minHeight: '100vh',
            background: 'linear-gradient(45deg, #f3e8e8 15%, #fef2f2 100%, #fdfdf9 100%, #f6f6f2 100%)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: '16px',
            overflow: 'hidden',
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
            textAlign: 'center',
            border: '1px solid #f3f4f6'

        },
        header: {
            marginBottom: '24px'
        },
        icon: {
            width: '64px',
            height: '64px',
            color: '#ef4444',
            margin: '0 auto 16px',
            textShadow: '0 0 4px rgba(239, 68, 68, 0.3)'

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
        warningBox: {
            backgroundColor: '#fffbeb',
            border: '1px solid #fde68a',
            borderRadius: '8px',
            padding: '16px',
            marginBottom: '24px'
        },
        warningContent: {
            display: 'flex',
            alignItems: 'flex-start'
        },
        warningIcon: {
            width: '20px',
            height: '20px',
            color: '#d97706',
            marginTop: '2px',
            marginRight: '12px',
            flexShrink: 0
        },
        warningText: {
            textAlign: 'left'
        },
        warningTitle: {
            fontWeight: '600',
            color: '#92400e',
            fontSize: '14px',
            marginBottom: '4px'
        },
        warningDescription: {
            color: '#b45309',
            fontSize: '12px'
        },
        buttonContainer: {
            display: 'flex',
            flexDirection: 'column',
            gap: '12px'
        },
        primaryButton: {
            width: '100%',
            backgroundColor: '#2563eb',
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
            backgroundColor: '#1d4ed8'
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
            transition: 'background-color 0.2s',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        },
        secondaryButtonHover: {
            backgroundColor: '#e5e7eb'
        },
        tertiaryButton: {
            width: '100%',
            backgroundColor: 'transparent',
            color: '#374151',
            fontWeight: '600',
            padding: '12px 24px',
            borderRadius: '8px',
            border: '1px solid #d1d5db',
            cursor: 'pointer',
            transition: 'background-color 0.2s',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        },
        tertiaryButtonHover: {
            backgroundColor: '#f9fafb'
        },
        buttonIcon: {
            width: '16px',
            height: '16px',
            marginRight: '8px'
        },
        buttonIconRight: {
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
            color: '#2563eb',
            textDecoration: 'none',
            marginLeft: '4px'
        }
    };

    return (
        <div style={styles.container}>
            <div style={styles.card}>
                <div style={styles.header}>
                    <XCircle style={styles.icon} />
                    <h1 style={styles.title}>
                        An error occured during payment
                    </h1>
                    <p style={styles.subtitle}>
                        Your subscription payment was cancelled. Don't worry, no charges were made to your account.
                    </p>
                </div>

                <div style={styles.warningBox}>
                    <div style={styles.warningContent}>
                        <MessageCircle style={styles.warningIcon} />
                        <div style={styles.warningText}>
                            <h3 style={styles.warningTitle}>
                                What happened?
                            </h3>
                            <p style={styles.warningDescription}>
                                The payment process was interrupted or cancelled before completion. Your subscription remains inactive.
                            </p>
                        </div>
                    </div>
                </div>

                <div style={styles.buttonContainer}>
                    <button
                        style={styles.primaryButton}
                        onClick={startCheckout}
                        onMouseOver={e => e.target.style.backgroundColor = styles.primaryButtonHover.backgroundColor}
                        onMouseOut={e => e.target.style.backgroundColor = styles.primaryButton.backgroundColor}
                    >
                        <RefreshCw style={styles.buttonIcon} />
                        Try Again
                    </button>

                    <button
                        style={styles.tertiaryButton}
                        onClick={() => navigate('/') }
                        onMouseOver={e => e.target.style.backgroundColor = styles.tertiaryButtonHover.backgroundColor}
                        onMouseOut={e => e.target.style.backgroundColor = styles.tertiaryButton.backgroundColor}
                    >
                        <ArrowLeft style={styles.buttonIcon} />
                        Back to Home
                    </button>
                </div>

                <div style={styles.footer}>
                    <p style={styles.footerText}>
                        Having trouble? <a href="/support" style={styles.footerLink}>Get Help</a> or
                        <a href="/contact" style={styles.footerLink}>Contact Us</a>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default SubscriptionCancel;