import React from 'react';

interface OverlapWarningProps {
    show: boolean;
}

export const OverlapWarning: React.FC<OverlapWarningProps> = ({ show }) => {
    if (!show) return null;

    return (
        <div className="animate-in fade-in rounded-lg border border-destructive/50 bg-destructive/10 p-4">
            <div className="flex items-center gap-2">
                <span className="text-lg">⚠️</span>
                <div>
                    <h4 className="text-sm font-semibold text-destructive">Time Slot Conflict</h4>
                    <p className="text-sm text-destructive/80">
                        The selected time range overlaps with an existing appointment for this owner.
                        Please choose a different time slot.
                    </p>
                </div>
            </div>
        </div>
    );
};
