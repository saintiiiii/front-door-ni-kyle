import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { Status } from './ui';

describe('FIXIT status labels', () => {
  it('shows the human-readable booking lifecycle state', () => {
    render(<Status value="WorkCompleted"/>);
    expect(screen.getByText('Work Completed')).toBeTruthy();
  });
  it('shows accepted appointments as accepted', () => {
    render(<Status value="Accepted"/>);
    expect(screen.getByText('Accepted')).toBeTruthy();
  });
});
