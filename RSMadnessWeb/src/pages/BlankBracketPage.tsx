export default function BlankBracketPage() {
  return (
    <div style={{ maxWidth: '720px' }}>
      <h2 style={{ marginBottom: '0.75rem' }}>
        “Every company has one Excel file that, if deleted, would cause total chaos.”
      </h2>
      <p style={{ color: '#64748b', marginBottom: '1.25rem' }}>
        Download the original spreadsheet that this application logic is based on.
      </p>

      <a
        href="/Blank Bracket 2026.xlsx"
        download="Blank Bracket 2026.xlsx"
        style={{
          display: 'inline-block',
          background: 'var(--orange)',
          color: '#0f172a',
          textDecoration: 'none',
          padding: '0.65rem 1rem',
          borderRadius: '6px',
          fontWeight: 700,
        }}
      >
        Download Blank Bracket
      </a>
    </div>
  );
}
