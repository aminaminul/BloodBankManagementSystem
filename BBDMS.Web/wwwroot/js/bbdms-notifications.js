// Emergency Audio Chime (Web Audio API)
function playEmergencyChime() {
    try {
        const AudioContext = window.AudioContext || window.webkitAudioContext;
        if (!AudioContext) return;
        const ctx = new AudioContext();

        const osc = ctx.createOscillator();
        const gain = ctx.createGain();

        osc.type = 'sine';
        osc.frequency.setValueAtTime(880, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(1046.5, ctx.currentTime + 0.12);

        gain.gain.setValueAtTime(0.2, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.45);

        osc.connect(gain);
        gain.connect(ctx.destination);

        osc.start();
        osc.stop(ctx.currentTime + 0.45);
    } catch (e) {
    }
}

// Notification Counter
async function fetchUnreadNotificationsCount() {
    try {
        const res = await fetch('/api/notification/unread-count');
        if (!res.ok) return;
        const data = await res.json();
        const badge = document.getElementById('notificationBadge');
        if (badge) {
            if (data.count > 0) {
                badge.textContent = data.count > 99 ? '99+' : data.count;
                badge.style.display = 'inline-block';
            } else {
                badge.style.display = 'none';
            }
        }
    } catch (e) {
    }
}

// Notification Dropdown List
async function loadNotificationsList() {
    try {
        const res = await fetch('/api/notification/recent');
        if (!res.ok) return;
        const items = await res.json();
        const container = document.getElementById('notificationList');
        if (!container) return;

        if (!items || items.length === 0) {
            container.innerHTML = `
                <div class="text-center py-4 text-muted small">
                    <i class="far fa-bell-slash fa-2x mb-2 d-block text-black-50"></i>
                    No notifications yet
                </div>`;
            return;
        }

        container.innerHTML = items.map(n => `
            <a href="${n.targetUrl || '#'}" class="dropdown-item p-3 border-bottom text-wrap ${n.isRead ? 'bg-white' : 'bg-light font-weight-bold'}" style="white-space: normal;" onclick="markNotificationRead(${n.id})">
                <div class="d-flex align-items-start">
                    <div class="mr-2 mt-1">
                        ${getCategoryIcon(n.category, n.urgency)}
                    </div>
                    <div class="flex-grow-1">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <span class="small font-weight-bold ${n.urgency === 'P0_Critical' ? 'text-danger' : 'text-dark'}">${escapeHtml(n.title)}</span>
                            <small class="text-muted" style="font-size: 0.75rem;">${n.timeAgo}</small>
                        </div>
                        <p class="small text-muted mb-0 font-weight-normal" style="font-size: 0.8rem; line-height: 1.3;">${escapeHtml(n.message)}</p>
                    </div>
                </div>
            </a>
        `).join('');
    } catch (e) {
    }
}

function getCategoryIcon(cat, urgency) {
    if (urgency === 'P0_Critical') {
        return '<span class="badge badge-danger p-2 rounded-circle"><i class="fas fa-exclamation-triangle"></i></span>';
    }
    switch (cat) {
        case 'Ambulance':
            return '<span class="badge badge-warning text-dark p-2 rounded-circle"><i class="fas fa-ambulance"></i></span>';
        case 'Milestone':
            return '<span class="badge badge-success p-2 rounded-circle"><i class="fas fa-award"></i></span>';
        default:
            return '<span class="badge badge-primary p-2 rounded-circle"><i class="fas fa-tint"></i></span>';
    }
}

// Mark Notification Read
async function markNotificationRead(id) {
    try {
        await fetch(`/api/notification/mark-read/${id}`, { method: 'POST' });
        fetchUnreadNotificationsCount();
    } catch (e) {}
}

async function markAllNotificationsRead() {
    try {
        await fetch('/api/notification/mark-all-read', { method: 'POST' });
        fetchUnreadNotificationsCount();
        loadNotificationsList();
    } catch (e) {}
}

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
}

// SignalR Realtime Listener
document.addEventListener('DOMContentLoaded', function () {
    fetchUnreadNotificationsCount();

    const bellBtn = document.getElementById('notificationBellBtn');
    if (bellBtn) {
        bellBtn.addEventListener('click', function () {
            loadNotificationsList();
        });
    }

    if (window.signalR && !window.emergencyHubNotificationWired) {
        window.emergencyHubNotificationWired = true;
        try {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/emergencyHub")
                .withAutomaticReconnect()
                .build();

            connection.on("ReceiveBloodRequestAlert", function (data) {
                playEmergencyChime();
                fetchUnreadNotificationsCount();
            });

            connection.on("ReceiveAmbulanceAlert", function (data) {
                playEmergencyChime();
                fetchUnreadNotificationsCount();
            });

            connection.start().catch(err => console.log('SignalR EmergencyHub connect error:', err));
        } catch (e) {}
    }
});
