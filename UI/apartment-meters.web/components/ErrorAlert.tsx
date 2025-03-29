import React, { useEffect } from 'react';
import { useError } from '../contexts/ErrorContext';

// Настройки автоматического скрытия ошибки
const AUTO_HIDE_DELAY = 7000; // 7 секунд для автоматического скрытия ошибки

/**
 * Компонент для отображения ошибок и уведомлений
 * Поддерживает разные типы уведомлений: error, warning, info, success
 */
const ErrorAlert: React.FC = () => {
  const { error, severity, clearError } = useError();

  // Автоматическое скрытие ошибки через заданное время
  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => {
        clearError();
      }, AUTO_HIDE_DELAY);
      
      return () => clearTimeout(timer);
    }
  }, [error, clearError]);

  // Если нет ошибки, не отображаем компонент
  if (!error) return null;

  // Определяем стили в зависимости от типа уведомления
  let backgroundColor: string;
  let borderColor: string;
  let textColor: string;
  let gradientFrom: string;
  let gradientTo: string;

  switch (severity) {
    case 'error':
      backgroundColor = '#FEF2F2';
      borderColor = '#F87171';
      textColor = '#B91C1C';
      gradientFrom = '#FEE2E2';
      gradientTo = '#FEF2F2';
      break;
    case 'warning':
      backgroundColor = '#FFFBEB';
      borderColor = '#FCD34D';
      textColor = '#92400E';
      gradientFrom = '#FEF3C7';
      gradientTo = '#FFFBEB';
      break;
    case 'info':
      backgroundColor = '#EFF6FF';
      borderColor = '#93C5FD';
      textColor = '#1E40AF';
      gradientFrom = '#DBEAFE';
      gradientTo = '#EFF6FF';
      break;
    case 'success':
      backgroundColor = '#ECFDF5';
      borderColor = '#6EE7B7';
      textColor = '#065F46';
      gradientFrom = '#D1FAE5';
      gradientTo = '#ECFDF5';
      break;
    default:
      backgroundColor = '#FEF2F2';
      borderColor = '#F87171';
      textColor = '#B91C1C';
      gradientFrom = '#FEE2E2';
      gradientTo = '#FEF2F2';
  }

  // SVG иконки для каждого типа уведомления
  const getIcon = () => {
    switch (severity) {
      case 'error':
        return (
          <svg xmlns="http://www.w3.org/2000/svg" className="icon" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
          </svg>
        );
      case 'warning':
        return (
          <svg xmlns="http://www.w3.org/2000/svg" className="icon" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
          </svg>
        );
      case 'info':
        return (
          <svg xmlns="http://www.w3.org/2000/svg" className="icon" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
          </svg>
        );
      case 'success':
        return (
          <svg xmlns="http://www.w3.org/2000/svg" className="icon" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
          </svg>
        );
      default:
        return (
          <svg xmlns="http://www.w3.org/2000/svg" className="icon" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
          </svg>
        );
    }
  };

  return (
    <div
      style={{
        position: 'fixed',
        top: '1rem',
        left: '50%',
        transform: 'translateX(-50%)',
        zIndex: 9999,
        padding: '0.85rem 1.2rem',
        borderRadius: '0.5rem',
        border: `1px solid ${borderColor}`,
        backgroundColor,
        background: `linear-gradient(to right, ${gradientFrom}, ${gradientTo})`,
        color: textColor,
        boxShadow: '0 4px 15px rgba(0, 0, 0, 0.1), 0 1px 3px rgba(0, 0, 0, 0.05)',
        display: 'flex',
        alignItems: 'center',
        maxWidth: '90%',
        width: 'auto',
        minWidth: '320px',
        animation: 'alertEntrance 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) forwards',
        transition: 'all 0.3s ease'
      }}
    >
      <style jsx global>{`
        @keyframes alertEntrance {
          0% {
            opacity: 0;
            transform: translate(-50%, -30px) scale(0.95);
          }
          50% {
            opacity: 0.9;
            transform: translate(-50%, 5px) scale(1);
          }
          100% {
            opacity: 1;
            transform: translate(-50%, 0) scale(1);
          }
        }
        
        .icon {
          width: 1.5rem;
          height: 1.5rem;
        }
        
        .close-button {
          opacity: 0.6;
          transition: all 0.2s ease;
        }
        
        .close-button:hover {
          opacity: 1;
          transform: scale(1.1);
        }
      `}</style>
      
      <div style={{ 
        marginRight: '0.75rem', 
        display: 'flex', 
        alignItems: 'center',
        color: textColor,
        backgroundColor: borderColor,
        padding: '0.5rem',
        borderRadius: '50%',
        boxShadow: '0 2px 5px rgba(0, 0, 0, 0.1)'
      }}>
        {getIcon()}
      </div>
      
      <div style={{ 
        flex: 1, 
        fontSize: '0.95rem', 
        fontWeight: 500,
        lineHeight: '1.4',
        paddingRight: '0.5rem'
      }}>
        {error}
      </div>
      
      <button
        onClick={clearError}
        className="close-button"
        style={{
          background: 'none',
          border: 'none',
          cursor: 'pointer',
          fontSize: '1.25rem',
          lineHeight: 1,
          color: textColor,
          marginLeft: '0.5rem',
          padding: '0.25rem',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          borderRadius: '50%'
        }}
        aria-label="Закрыть"
      >
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          <line x1="18" y1="6" x2="6" y2="18"></line>
          <line x1="6" y1="6" x2="18" y2="18"></line>
        </svg>
      </button>
    </div>
  );
};

export default ErrorAlert; 