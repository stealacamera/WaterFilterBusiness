// ----------------------------------------------------------------------

function path(root, sublink) {
  return `${root}${sublink}`;
}

const ROOTS_AUTH = '/auth';
const ROOTS_DASHBOARD = '/';

// ----------------------------------------------------------------------

export const PATH_AUTH = {
  root: ROOTS_AUTH,
  login: path(ROOTS_AUTH, '/login'),
  register: path(ROOTS_AUTH, '/register'),
  loginUnprotected: path(ROOTS_AUTH, '/login-unprotected'),
  registerUnprotected: path(ROOTS_AUTH, '/register-unprotected'),
  verify: path(ROOTS_AUTH, '/verify'),
  resetPassword: path(ROOTS_AUTH, '/reset-password'),
};

export const PATH_PAGE = {
  comingSoon: '/coming-soon',
  maintenance: '/maintenance',
  pricing: '/pricing',
  payment: '/payment',
  about: '/about-us',
  contact: '/contact-us',
  faqs: '/faqs',
  page404: '/404',
  page500: '/500',
  components: '/components',
};

export const PATH_DASHBOARD = {
  root: ROOTS_DASHBOARD,
  general: {
    dashboard: '/dashboard',
    ecommerce: '/ecommerce',
    analytics: '/analytics',
    banking: '/banking',
    meeting: '/meeting',
    newMeeting: '/meeting/new',
  },
  calls: {
    root: '/calls',
    list: '/calls/list',
    new: '/calls/new',
    edit: (name) => `/calls/${name}/edit`,
    // demoEdit: `calls/nike-blazer-low-77-vintage/edit`,
  },
  mail: {
    root: '/mail',
    all: '/mail/all',
  },
  chat: {
    root: '/chat',
    new: '/chat/new',
    view: (name) => `/chat/${name}`,
  },
  schedule: '/schedule',
  kanban: '/kanban',
  user: {
    root: '/user',
    new: '/user/new',
    list: '/user/list',
    cards: '/user/cards',
    profile: '/user/profile',
    account: '/user/account',
    edit: (name) => `/user/${name}/edit`,
    demoEdit: `/user/reece-chung/edit`,
  },
  eCommerce: {
    root: '/e-commerce',
    shop: '/e-commerce/shop',
    list: '/e-commerce/list',
    checkout: '/e-commerce/checkout',
    new: '/e-commerce/product/new',
    view: (name) => `/e-commerce/product/${name}`,
    edit: (name) => `/e-commerce/product/${name}/edit`,
    demoEdit: '/e-commerce/product/nike-blazer-low-77-vintage/edit',
    demoView: '/e-commerce/product/nike-air-force-1-ndestrukt',
  },
  invoice: {
    root: '/invoice',
    list: '/invoice/list',
    new: '/invoice/new',
    view: (id) => `/invoice/${id}`,
    edit: (id) => `/invoice/${id}/edit`,
    demoEdit: '/invoice/e99f09a7-dd88-49d5-b1c8-1daf80c2d7b1/edit',
    demoView: '/invoice/e99f09a7-dd88-49d5-b1c8-1daf80c2d7b5',
  },
  blog: {
    root: '/blog',
    posts: '/blog/posts',
    new: '/blog/new',
    view: (title) => `/blog/post/${title}`,
    demoView: '/blog/post/apply-these-7-secret-techniques-to-improve-event',
  },
};

export const PATH_DOCS = 'https://docs-minimals.vercel.app/introduction';
