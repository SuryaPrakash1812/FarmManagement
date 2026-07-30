import { useEffect, useMemo, useRef, useState } from 'react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { Alert, AppBar, Box, Button, CssBaseline, Dialog, DialogActions, DialogContent, DialogTitle, Drawer, IconButton, Snackbar, Toolbar, Typography } from '@mui/material';
import { Add, CameraAlt, CloudUpload, Dashboard, DarkMode, Delete, Edit, Group, Inventory, Menu, Payments, Pets, ReceiptLong, Search, Settings, Visibility, WbSunny } from '@mui/icons-material';
import { Bar, BarChart, CartesianGrid, Legend, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis, Cell } from 'recharts';
import { api, login } from './api/client.js';

const modules = [
  ['dashboard','Dashboard',Dashboard],['animals','Animals',Pets],['stock','Stock',Inventory],['sales','Sales',ReceiptLong],['expenses','Expenses',Payments],['income','Income',Payments],['payments','Payments',Payments],['health','Health',Pets],['breeding','Breeding',Pets],['employees','Employees',Settings],['team','Team',Group,true],['reports','Reports',ReceiptLong],['settings','Settings',Settings]
];
const roleNames=['Admin','Manager','Worker'];
const colors=['#2563eb','#10b981','#f59e0b','#ef4444','#8b5cf6','#14b8a6'];

export default function App(){
  const [page,setPage]=useState('dashboard'); const [drawer,setDrawer]=useState(false); const [dark,setDark]=useState(false); const [toast,setToast]=useState('');
  const [authed,setAuthed]=useState(!!localStorage.getItem('farm_token'));
  const [currentUser,setCurrentUser]=useState(()=>{try{return JSON.parse(localStorage.getItem('farm_user')||'null')}catch{return null}});
  const theme=useMemo(()=>createTheme({palette:{mode:dark?'dark':'light',primary:{main:'#2563eb'},success:{main:'#10b981'}},shape:{borderRadius:8}}),[dark]);
  if(!authed) return <ThemeProvider theme={theme}><CssBaseline/><LoginPage onLogin={(user)=>{setCurrentUser(user);setAuthed(true)}}/></ThemeProvider>;
  const isAdmin=currentUser?.role===0;
  const visibleModules=modules.filter(m=>!m[3]||isAdmin);
  const Nav= <nav>{visibleModules.map(([key,label,Icon])=><button key={key} className={`nav-btn ${page===key?'active':''}`} onClick={()=>{setPage(key);setDrawer(false)}}><Icon fontSize="small"/>{label}</button>)}</nav>;
  function logout(){localStorage.removeItem('farm_token');localStorage.removeItem('farm_user');setCurrentUser(null);setAuthed(false)}
  return <ThemeProvider theme={theme}><CssBaseline/><div className={dark?'dark':''}><div className="app-shell"><aside className="sidebar"> <div className="brand">Farm Management</div>{Nav}</aside><Drawer open={drawer} onClose={()=>setDrawer(false)}><Box sx={{width:280,background:'#0f172a',height:'100%',color:'white',p:2}}><div className="brand">Farm Management</div>{Nav}</Box></Drawer><main className="content"><AppBar position="sticky" color="inherit" elevation={0}><Toolbar><IconButton className="mobile-menu" onClick={()=>setDrawer(true)}><Menu/></IconButton><Typography variant="h6" sx={{flex:1,textTransform:'capitalize'}}>{page}</Typography>{currentUser && <Typography variant="body2" className="muted" sx={{mr:1}}>{currentUser.fullName} · {roleNames[currentUser.role]||'User'}</Typography>}<IconButton onClick={()=>setDark(!dark)}>{dark?<WbSunny/>:<DarkMode/>}</IconButton><Button size="small" onClick={logout} sx={{ml:1}}>Logout</Button></Toolbar></AppBar><section className="main"><Page page={page} setToast={setToast} currentUser={currentUser}/></section></main></div><Snackbar open={!!toast} autoHideDuration={2800} onClose={()=>setToast('')}><Alert severity="success">{toast}</Alert></Snackbar></div></ThemeProvider>
}

function LoginPage({onLogin}){
  const [email,setEmail]=useState(''); const [password,setPassword]=useState(''); const [error,setError]=useState(''); const [loading,setLoading]=useState(false);
  async function submit(e){
    e.preventDefault(); setError(''); setLoading(true);
    try{ const data=await login(email,password); onLogin(data.user); }
    catch(err){ setError(err?.response?.status===401?'Invalid email or password.':'Could not reach the server. Please try again.'); }
    finally{ setLoading(false); }
  }
  return <div className="login-wrap"><form className="card login-card" onSubmit={submit}>
    <div className="brand" style={{marginBottom:8}}>Farm Management</div>
    <Typography variant="body2" className="muted" sx={{mb:2}}>Sign in to continue</Typography>
    {error && <Alert severity="error" sx={{mb:2}}>{error}</Alert>}
    <input className="input" type="email" placeholder="Email" autoComplete="username" value={email} onChange={e=>setEmail(e.target.value)} required style={{marginBottom:12,width:'100%'}}/>
    <input className="input" type="password" placeholder="Password" autoComplete="current-password" value={password} onChange={e=>setPassword(e.target.value)} required style={{marginBottom:16,width:'100%'}}/>
    <Button type="submit" variant="contained" fullWidth disabled={loading}>{loading?'Signing in...':'Sign In'}</Button>
  </form></div>
}

function Page({page,setToast,currentUser}){ if(page==='dashboard')return <DashboardPage/>; if(page==='animals')return <AnimalsPage setToast={setToast}/>; if(page==='expenses')return <ExpensesPage setToast={setToast}/>; if(page==='employees')return <EmployeesPage setToast={setToast}/>; if(page==='team')return <TeamPage setToast={setToast} currentUser={currentUser}/>; if(page==='reports')return <ReportsPage/>; return <ModulePage name={page}/> }

function DashboardPage(){ const [data,setData]=useState(null); const [failed,setFailed]=useState(false); useEffect(()=>{api.get('/dashboard').then(r=>{setData(r.data);setFailed(false)}).catch(()=>{setData(sampleDashboard);setFailed(true)})},[]); const d=data||sampleDashboard; return <div className="grid">{failed && <Alert severity="warning">Couldn't load live data â€” showing sample values.</Alert>}<div className="grid stats">{[['Animals',d.totalAnimals],['Stock',d.totalStock],['Income',money(d.monthlyIncome)],['Expenses',money(d.monthlyExpenses)],['Pending',money(d.pendingPayments)],['Today Sales',money(d.todaysSales)]].map(x=><div className="card" key={x[0]}><div className="muted">{x[0]}</div><div className="metric">{x[1]}</div></div>)}</div><div className="grid two"><div className="card"><h3>Income vs Expenses</h3><ResponsiveContainer width="100%" height={280}><BarChart data={d.incomeVsExpenses}><CartesianGrid strokeDasharray="3 3"/><XAxis dataKey="label"/><YAxis/><Tooltip/><Legend/><Bar dataKey="value" name="Income" fill="#10b981"/><Bar dataKey="secondaryValue" name="Expenses" fill="#ef4444"/></BarChart></ResponsiveContainer></div><div className="card"><h3>Animals by Category</h3><ResponsiveContainer width="100%" height={280}><PieChart><Pie data={d.animalCountByCategory} dataKey="value" nameKey="label" outerRadius={92} label>{d.animalCountByCategory?.map((_,i)=><Cell key={i} fill={colors[i%colors.length]}/>)}</Pie><Tooltip/></PieChart></ResponsiveContainer></div></div><div className="card"><h3>Recent Activities</h3>{d.recentActivities?.map((a,i)=><p key={i}><b>{a.action}</b> <span className="muted">{a.entityName}</span></p>)}</div></div> }

function AnimalsPage({setToast}){ const [items,setItems]=useState([]); const [open,setOpen]=useState(false); const [editing,setEditing]=useState(null); const [search,setSearch]=useState(''); const [viewPhoto,setViewPhoto]=useState(null); const [confirmDelete,setConfirmDelete]=useState(null); useEffect(()=>{load()},[]); async function load(){try{const r=await api.get('/animals',{params:{search}});setItems(r.data.items||[])}catch{setItems(sampleAnimals)}} async function remove(a){try{await api.delete(`/animals/${a.id}`);setToast('Animal deleted');load()}catch{setToast('Could not delete animal')}finally{setConfirmDelete(null)}} return <><div className="toolbar"><div style={{display:'flex',gap:8}}><Search/><input className="input" placeholder="Search tag, name, species" value={search} onChange={e=>setSearch(e.target.value)} onKeyDown={e=>e.key==='Enter'&&load()}/></div><Button startIcon={<Add/>} variant="contained" onClick={()=>{setEditing(null);setOpen(true)}}>Add Animal</Button></div><AnimalTable items={items} onView={setViewPhoto} onEdit={a=>{setEditing(a);setOpen(true)}} onDelete={setConfirmDelete}/><AnimalDialog open={open} animal={editing} onClose={()=>setOpen(false)} onSaved={()=>{setToast(editing?'Animal updated':'Animal saved');setOpen(false);load()}}/><Dialog open={!!viewPhoto} onClose={()=>setViewPhoto(null)} maxWidth="sm" fullWidth><DialogTitle>Photo</DialogTitle><DialogContent sx={{textAlign:'center'}}><img src={viewPhoto} style={{maxWidth:'100%',borderRadius:8}}/></DialogContent><DialogActions><Button onClick={()=>setViewPhoto(null)}>Close</Button></DialogActions></Dialog><Dialog open={!!confirmDelete} onClose={()=>setConfirmDelete(null)}><DialogTitle>Delete {confirmDelete?.name}?</DialogTitle><DialogContent><Typography className="muted">This can't be undone.</Typography></DialogContent><DialogActions><Button onClick={()=>setConfirmDelete(null)}>Cancel</Button><Button color="error" variant="contained" onClick={()=>remove(confirmDelete)}>Delete</Button></DialogActions></Dialog></> }
function AnimalTable({items,onView,onEdit,onDelete}){return <div className="card table-wrap"><table className="table"><thead><tr><th>Photo</th><th>Tag</th><th>Name</th><th>Species</th><th>Breed</th><th>Status</th><th>Value</th><th>Actions</th></tr></thead><tbody>{items.map(a=><tr key={a.id||a.tagNumber}><td data-label="Photo">{a.photoUrl?<img src={a.photoUrl} style={{width:48,height:48,borderRadius:8,objectFit:'cover',cursor:'pointer'}} onClick={()=>onView(a.photoUrl)}/>:'-'}</td><td data-label="Tag">{a.tagNumber}</td><td data-label="Name">{a.name}</td><td data-label="Species">{a.species}</td><td data-label="Breed">{a.breed}</td><td data-label="Status">{a.status}</td><td data-label="Value">{money(a.currentValue)}</td><td data-label="Actions"><IconButton size="small" disabled={!a.photoUrl} onClick={()=>onView(a.photoUrl)} title="View photo"><Visibility fontSize="small"/></IconButton><IconButton size="small" onClick={()=>onEdit(a)} title="Edit"><Edit fontSize="small"/></IconButton><IconButton size="small" onClick={()=>onDelete(a)} title="Delete"><Delete fontSize="small" color="error"/></IconButton></td></tr>)}</tbody></table></div>}
function AnimalDialog({open,animal,onClose,onSaved}){ const blank={animalCode:'',tagNumber:'',name:'',species:'Cow',breed:'',gender:0,purchasePrice:0,currentValue:0,healthStatus:'Healthy',isPregnant:false,status:0}; const [form,setForm]=useState(blank); const [photo,setPhoto]=useState(null); const [preview,setPreview]=useState(''); const video=useRef(null); const streamRef=useRef(null); useEffect(()=>{if(open){setForm(animal?{...blank,...animal}:blank);setPreview(animal?.photoUrl||'');setPhoto(null)}},[open,animal]); function patch(e){setForm({...form,[e.target.name]:e.target.value})} function pick(file){if(!file)return;setPhoto(file);setPreview(URL.createObjectURL(file))} async function startCamera(){streamRef.current=await navigator.mediaDevices.getUserMedia({video:{facingMode:'environment'}});video.current.srcObject=streamRef.current} function capture(){const c=document.createElement('canvas');c.width=video.current.videoWidth;c.height=video.current.videoHeight;c.getContext('2d').drawImage(video.current,0,0);c.toBlob(b=>pick(new File([b],'animal-camera.jpg',{type:'image/jpeg'})),'image/jpeg',.92)} async function save(){let saved; try{saved=animal?(await api.put(`/animals/${animal.id}`,form)).data:(await api.post('/animals',form)).data;if(photo){const fd=new FormData();fd.append('photo',photo);await api.post(`/animals/${saved.id}/photo`,fd,{headers:{'Content-Type':'multipart/form-data'}})}}catch{} stop(); onSaved()} function stop(){streamRef.current?.getTracks().forEach(t=>t.stop())} return <Dialog open={open} onClose={()=>{stop();onClose()}} fullWidth maxWidth="md"><DialogTitle>{animal?'Edit Animal':'Add Animal'}</DialogTitle><DialogContent><div className="form-grid">{['animalCode','tagNumber','name','species','breed','healthStatus'].map(k=><input key={k} className="input" name={k} placeholder={label(k)} value={form[k]} onChange={patch}/>) }<input className="input" name="purchasePrice" type="number" placeholder="Purchase Price" value={form.purchasePrice} onChange={patch}/><input className="input" name="currentValue" type="number" placeholder="Current Value" value={form.currentValue} onChange={patch}/><select name="gender" value={form.gender} onChange={patch}><option value={0}>Female</option><option value={1}>Male</option><option value={2}>Unknown</option></select></div><Box sx={{mt:2}} className="photo-panel"><img className="photo-preview" src={preview||'/icon.svg'} /><div><Button component="label" startIcon={<CloudUpload/>} variant="outlined" sx={{mr:1,mb:1}}>Upload Photo<input hidden type="file" accept="image/*" onChange={e=>pick(e.target.files[0])}/></Button><Button component="label" startIcon={<CameraAlt/>} variant="outlined" sx={{mr:1,mb:1}}>Take Pic<input hidden type="file" accept="image/*" capture="environment" onChange={e=>pick(e.target.files[0])}/></Button><Button startIcon={<CameraAlt/>} variant="contained" onClick={startCamera} sx={{mb:1}}>Live Camera</Button><video ref={video} autoPlay playsInline style={{width:'100%',maxWidth:360,borderRadius:8,display:'block',marginTop:8}}/><Button className="btn secondary" onClick={capture} sx={{mt:1}}>Capture Frame</Button></div></Box></DialogContent><DialogActions><Button onClick={()=>{stop();onClose()}}>Cancel</Button><Button variant="contained" onClick={save}>Save</Button></DialogActions></Dialog>}
const expenseCategories=['Feed','Medicine','Labor','Equipment','Utilities','Transport','Veterinary','Maintenance','Other'];
const paymentMethods=['Cash','UPI','Bank Transfer','Card','Credit'];

function ExpensesPage({setToast}){
  const [items,setItems]=useState([]); const [users,setUsers]=useState([]); const [balances,setBalances]=useState([]); const [suggestions,setSuggestions]=useState([]);
  const [open,setOpen]=useState(false); const [editing,setEditing]=useState(null); const [confirmDelete,setConfirmDelete]=useState(null);
  const [personFilter,setPersonFilter]=useState(''); const [recurringOnly,setRecurringOnly]=useState(false); const [settleTarget,setSettleTarget]=useState(null);
  useEffect(()=>{loadUsers();loadBalances()},[]);
  useEffect(()=>{load()},[personFilter,recurringOnly]);
  async function loadUsers(){try{const r=await api.get('/auth/users');setUsers(r.data||[])}catch{setUsers([])}}
  async function load(){try{const params={}; if(personFilter)params.personId=personFilter; if(recurringOnly)params.recurring=true; const r=await api.get('/expenses',{params}); setItems(r.data.items||[])}catch{setItems(sampleExpenses)}}
  async function loadBalances(){try{const r=await api.get('/expenses/balances');setBalances(r.data||[])}catch{setBalances([])} try{const r2=await api.get('/expenses/settle-suggestions');setSuggestions(r2.data||[])}catch{setSuggestions([])}}
  async function remove(x){try{await api.delete(`/expenses/${x.id}`);setToast('Expense deleted');load();loadBalances()}catch{setToast('Could not delete expense')}finally{setConfirmDelete(null)}}
  async function recordSettlement(s){try{await api.post('/expenses/settlements',{fromUserId:s.fromUserId,toUserId:s.toUserId,amount:s.amount,date:new Date().toISOString().slice(0,10),notes:'Settled up'});setToast('Settlement recorded')}catch{setToast('Could not record settlement')}finally{setSettleTarget(null);loadBalances()}}
  return <div className="grid">
    <div className="grid two">
      <div className="card">
        <h3>Balances</h3>
        {balances.length===0 && <p className="muted">No balance data yet.</p>}
        <div style={{display:'flex',flexWrap:'wrap',gap:12}}>
          {balances.map(b=><div key={b.userId} className="card" style={{flex:'1 1 140px'}}>
            <div className="muted">{b.fullName}</div>
            <div className="metric" style={{color:b.netBalance>0.01?'#10b981':(b.netBalance<-0.01?'#ef4444':'inherit')}}>{money(Math.abs(b.netBalance))}</div>
            <div className="muted">{b.netBalance>0.01?'is owed':(b.netBalance<-0.01?'owes others':'settled up')}</div>
          </div>)}
        </div>
      </div>
      <div className="card">
        <h3>Settle Up</h3>
        {suggestions.length===0 && <p className="muted">Everyone's settled up.</p>}
        {suggestions.map((s,i)=><div key={i} style={{display:'flex',justifyContent:'space-between',alignItems:'center',padding:'8px 0',borderBottom:'1px solid #e5e7eb'}}>
          <span><b>{s.fromName}</b> owes <b>{s.toName}</b> {money(s.amount)}</span>
          <Button size="small" variant="outlined" onClick={()=>setSettleTarget(s)}>Settle</Button>
        </div>)}
      </div>
    </div>
    <div className="toolbar">
      <div style={{display:'flex',gap:8,flexWrap:'wrap'}}>
        <select className="input" style={{width:180}} value={personFilter} onChange={e=>setPersonFilter(e.target.value)}>
          <option value="">All people</option>
          {users.map(u=><option key={u.id} value={u.id}>{u.fullName}</option>)}
        </select>
        <Button variant={recurringOnly?'contained':'outlined'} onClick={()=>setRecurringOnly(!recurringOnly)}>Recurring only</Button>
      </div>
      <Button startIcon={<Add/>} variant="contained" onClick={()=>{setEditing(null);setOpen(true)}}>Add Expense</Button>
    </div>
    <div className="card table-wrap"><table className="table"><thead><tr><th>Date</th><th>Category</th><th>Amount</th><th>Paid By</th><th>Split</th><th>Method</th><th>Recurring</th><th>Actions</th></tr></thead><tbody>
      {items.map(x=><tr key={x.id}>
        <td data-label="Date">{x.date}</td>
        <td data-label="Category">{x.category}</td>
        <td data-label="Amount">{money(x.amount)}</td>
        <td data-label="Paid By">{x.paidByName||'-'}</td>
        <td data-label="Split">{x.splits?.length?x.splits.map(s=>`${s.userName} ${money(s.shareAmount)}`).join(', '):'-'}</td>
        <td data-label="Method">{x.paymentMethod}</td>
        <td data-label="Recurring">{x.isRecurring?(x.recurrenceInterval||'Yes'):'-'}</td>
        <td data-label="Actions"><IconButton size="small" onClick={()=>{setEditing(x);setOpen(true)}} title="Edit"><Edit fontSize="small"/></IconButton><IconButton size="small" onClick={()=>setConfirmDelete(x)} title="Delete"><Delete fontSize="small" color="error"/></IconButton></td>
      </tr>)}
    </tbody></table></div>
    <ExpenseDialog open={open} expense={editing} users={users} onClose={()=>setOpen(false)} onSaved={()=>{setToast(editing?'Expense updated':'Expense saved');setOpen(false);load();loadBalances()}}/>
    <Dialog open={!!confirmDelete} onClose={()=>setConfirmDelete(null)}><DialogTitle>Delete this expense?</DialogTitle><DialogContent><Typography className="muted">This can't be undone.</Typography></DialogContent><DialogActions><Button onClick={()=>setConfirmDelete(null)}>Cancel</Button><Button color="error" variant="contained" onClick={()=>remove(confirmDelete)}>Delete</Button></DialogActions></Dialog>
    <Dialog open={!!settleTarget} onClose={()=>setSettleTarget(null)}><DialogTitle>Record Settlement</DialogTitle><DialogContent>{settleTarget && <Typography>Mark that <b>{settleTarget.fromName}</b> paid <b>{settleTarget.toName}</b> {money(settleTarget.amount)}?</Typography>}</DialogContent><DialogActions><Button onClick={()=>setSettleTarget(null)}>Cancel</Button><Button variant="contained" onClick={()=>recordSettlement(settleTarget)}>Confirm</Button></DialogActions></Dialog>
  </div>
}

function ExpenseDialog({open,expense,users,onClose,onSaved}){
  const blank={category:'Feed',amount:0,paymentMethod:'Cash',date:new Date().toISOString().slice(0,10),notes:'',paidByUserId:'',isRecurring:false,recurrenceInterval:'Monthly'};
  const [form,setForm]=useState(blank); const [splitAmong,setSplitAmong]=useState([]);
  useEffect(()=>{ if(open){ if(expense){ setForm({category:expense.category,amount:expense.amount,paymentMethod:expense.paymentMethod,date:expense.date,notes:expense.notes||'',paidByUserId:expense.paidByUserId||'',isRecurring:expense.isRecurring,recurrenceInterval:expense.recurrenceInterval||'Monthly'}); setSplitAmong((expense.splits||[]).map(s=>s.userId)); } else { setForm(blank); setSplitAmong([]); } } },[open,expense]);
  function patch(e){const {name,value,type,checked}=e.target; setForm({...form,[name]:type==='checkbox'?checked:value})}
  function toggleSplit(id){setSplitAmong(splitAmong.includes(id)?splitAmong.filter(x=>x!==id):[...splitAmong,id])}
  async function save(){
    const payload={category:form.category,amount:Number(form.amount),paymentMethod:form.paymentMethod,date:form.date,notes:form.notes||null,paidByUserId:form.paidByUserId||null,isRecurring:!!form.isRecurring,recurrenceInterval:form.isRecurring?form.recurrenceInterval:null,splitAmongUserIds:splitAmong.length?splitAmong:null};
    try{ if(expense) await api.put(`/expenses/${expense.id}`,payload); else await api.post('/expenses',payload); }catch{}
    onSaved();
  }
  return <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
    <DialogTitle>{expense?'Edit Expense':'Add Expense'}</DialogTitle>
    <DialogContent>
      <div className="form-grid">
        <select className="input" name="category" value={form.category} onChange={patch}>{expenseCategories.map(c=><option key={c} value={c}>{c}</option>)}</select>
        <input className="input" name="amount" type="number" placeholder="Amount" value={form.amount} onChange={patch}/>
        <select className="input" name="paymentMethod" value={form.paymentMethod} onChange={patch}>{paymentMethods.map(m=><option key={m} value={m}>{m}</option>)}</select>
        <input className="input" name="date" type="date" value={form.date} onChange={patch}/>
        <select className="input" name="paidByUserId" value={form.paidByUserId} onChange={patch}><option value="">Paid by...</option>{users.map(u=><option key={u.id} value={u.id}>{u.fullName}</option>)}</select>
        <input className="input" name="notes" placeholder="Notes" value={form.notes} onChange={patch}/>
      </div>
      <Box sx={{mt:2}}>
        <Typography variant="body2" className="muted" sx={{mb:1}}>Split among (defaults to payer only if none selected)</Typography>
        <div style={{display:'flex',gap:8,flexWrap:'wrap'}}>
          {users.map(u=><Button key={u.id} size="small" variant={splitAmong.includes(u.id)?'contained':'outlined'} onClick={()=>toggleSplit(u.id)}>{u.fullName}</Button>)}
        </div>
      </Box>
      <Box sx={{mt:2,display:'flex',alignItems:'center',gap:12}}>
        <label style={{display:'flex',alignItems:'center',gap:6}}><input type="checkbox" name="isRecurring" checked={form.isRecurring} onChange={patch}/> Recurring</label>
        {form.isRecurring && <select className="input" style={{width:140}} name="recurrenceInterval" value={form.recurrenceInterval} onChange={patch}><option value="Weekly">Weekly</option><option value="Monthly">Monthly</option></select>}
      </Box>
    </DialogContent>
    <DialogActions><Button onClick={onClose}>Cancel</Button><Button variant="contained" onClick={save}>Save</Button></DialogActions>
  </Dialog>
}
const sampleExpenses=[{id:'s1',date:'2026-07-01',category:'Feed',amount:4200,paidByName:'Farm Admin',paymentMethod:'UPI',isRecurring:false,splits:[]}];
const employeeRoles=['Manager','Worker','Veterinarian','Accountant','Driver','Security','Part-time'];

function EmployeesPage({setToast}){
  const [items,setItems]=useState([]); const [attendance,setAttendance]=useState([]); const [search,setSearch]=useState(''); const [open,setOpen]=useState(false); const [editing,setEditing]=useState(null); const [confirmDelete,setConfirmDelete]=useState(null); const [loading,setLoading]=useState(false);
  const today=new Date().toISOString().slice(0,10);
  useEffect(()=>{load();loadAttendance()},[]);
  async function load(){setLoading(true);try{const r=await api.get('/employees',{params:{search,pageSize:100}});setItems(r.data.items||[])}catch{setItems(sampleEmployees)}finally{setLoading(false)}}
  async function loadAttendance(){try{const r=await api.get('/attendance',{params:{date:today}});setAttendance(r.data||[])}catch{setAttendance([])}}
  async function remove(emp){try{await api.delete(`/employees/${emp.id}`);setToast('Employee deleted');load();loadAttendance()}catch{setToast('Could not delete employee')}finally{setConfirmDelete(null)}}
  async function mark(emp,isPresent){try{await api.post('/attendance',{employeeId:emp.id,date:today,isPresent,notes:isPresent?'Checked in':'Absent'});setToast(isPresent?'Marked present':'Marked absent');loadAttendance()}catch{setToast('Could not mark attendance')}}
  const presentIds=new Set(attendance.filter(a=>a.isPresent).map(a=>a.employeeId));
  const absentIds=new Set(attendance.filter(a=>!a.isPresent).map(a=>a.employeeId));
  const monthlySalary=items.reduce((sum,e)=>sum+Number(e.salary||0),0);
  const assignedTasks=items.filter(e=>(e.tasks||'').trim()).length;
  return <div className="grid">
    <div className="grid stats employee-stats">
      <div className="card"><div className="muted">Employees</div><div className="metric">{items.length}</div></div>
      <div className="card"><div className="muted">Monthly Payroll</div><div className="metric">{money(monthlySalary)}</div></div>
      <div className="card"><div className="muted">Present Today</div><div className="metric">{presentIds.size}</div></div>
      <div className="card"><div className="muted">Absent Today</div><div className="metric">{absentIds.size}</div></div>
      <div className="card"><div className="muted">Assigned Tasks</div><div className="metric">{assignedTasks}</div></div>
    </div>
    <div className="toolbar">
      <div style={{display:'flex',gap:8,alignItems:'center'}}><Search/><input className="input" placeholder="Search employee or role" value={search} onChange={e=>setSearch(e.target.value)} onKeyDown={e=>e.key==='Enter'&&load()}/><Button variant="outlined" onClick={load}>Search</Button></div>
      <Button startIcon={<Add/>} variant="contained" onClick={()=>{setEditing(null);setOpen(true)}}>Add Employee</Button>
    </div>
    <div className="card table-wrap"><table className="table"><thead><tr><th>Name</th><th>Role</th><th>Salary</th><th>Phone</th><th>Address</th><th>Tasks</th><th>Attendance</th><th>Actions</th></tr></thead><tbody>
      {items.map(emp=><tr key={emp.id||emp.fullName}>
        <td data-label="Name"><b>{emp.fullName}</b></td>
        <td data-label="Role">{emp.role}</td>
        <td data-label="Salary">{money(emp.salary)}</td>
        <td data-label="Phone">{emp.phone||'-'}</td>
        <td data-label="Address">{emp.address||'-'}</td>
        <td data-label="Tasks"><span className="task-text">{emp.tasks||'-'}</span></td>
        <td data-label="Attendance"><div className="attendance-actions"><Button size="small" variant={presentIds.has(emp.id)?'contained':'outlined'} color="success" onClick={()=>mark(emp,true)}>Present</Button><Button size="small" variant={absentIds.has(emp.id)?'contained':'outlined'} color="error" onClick={()=>mark(emp,false)}>Absent</Button></div></td>
        <td data-label="Actions"><IconButton size="small" onClick={()=>{setEditing(emp);setOpen(true)}} title="Edit"><Edit fontSize="small"/></IconButton><IconButton size="small" onClick={()=>setConfirmDelete(emp)} title="Delete"><Delete fontSize="small" color="error"/></IconButton></td>
      </tr>)}
      {!loading && items.length===0 && <tr><td colSpan="8"><p className="muted">No employees found.</p></td></tr>}
    </tbody></table></div>
    <div className="card"><h3>Today&apos;s Attendance</h3>{items.length===0?<p className="muted">Add employees to start tracking attendance.</p>:<div className="attendance-list">{items.map(emp=>{const rec=attendance.find(a=>a.employeeId===emp.id);return <div key={emp.id} className="attendance-row"><span><b>{emp.fullName}</b><small>{emp.role}</small></span><span className={`status-pill ${rec?.isPresent?'present':rec?'absent':''}`}>{rec?(rec.isPresent?'Present':'Absent'):'Not marked'}</span></div>})}</div>}</div>
    <EmployeeDialog open={open} employee={editing} onClose={()=>setOpen(false)} onSaved={()=>{setToast(editing?'Employee updated':'Employee saved');setOpen(false);load()}}/>
    <Dialog open={!!confirmDelete} onClose={()=>setConfirmDelete(null)}><DialogTitle>Delete {confirmDelete?.fullName}?</DialogTitle><DialogContent><Typography className="muted">This employee will be soft deleted and hidden from lists.</Typography></DialogContent><DialogActions><Button onClick={()=>setConfirmDelete(null)}>Cancel</Button><Button color="error" variant="contained" onClick={()=>remove(confirmDelete)}>Delete</Button></DialogActions></Dialog>
  </div>
}

function EmployeeDialog({open,employee,onClose,onSaved}){
  const blank={fullName:'',role:'Worker',salary:0,phone:'',address:'',tasks:''}; const [form,setForm]=useState(blank); const [error,setError]=useState('');
  useEffect(()=>{if(open){setForm(employee?{...blank,...employee}:blank);setError('')}},[open,employee]);
  function patch(e){setForm({...form,[e.target.name]:e.target.value})}
  async function save(){if(!form.fullName.trim()){setError('Employee name is required.');return} if(!form.role.trim()){setError('Role is required.');return} const payload={...form,salary:Number(form.salary||0),phone:form.phone||null,address:form.address||null,tasks:form.tasks||null}; try{if(employee)await api.put(`/employees/${employee.id}`,payload);else await api.post('/employees',payload);onSaved()}catch{setError('Could not save employee. Check required fields and try again.')}}
  return <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm"><DialogTitle>{employee?'Edit Employee':'Add Employee'}</DialogTitle><DialogContent>{error&&<Alert severity="error" sx={{mb:2}}>{error}</Alert>}<div className="form-grid"><input className="input" name="fullName" placeholder="Full name" value={form.fullName} onChange={patch}/><select className="input" name="role" value={form.role} onChange={patch}>{employeeRoles.map(r=><option key={r} value={r}>{r}</option>)}</select><input className="input" name="salary" type="number" placeholder="Monthly salary" value={form.salary} onChange={patch}/><input className="input" name="phone" placeholder="Phone" value={form.phone||''} onChange={patch}/><input className="input" name="address" placeholder="Address" value={form.address||''} onChange={patch}/><textarea className="input" name="tasks" placeholder="Tasks and responsibilities" value={form.tasks||''} onChange={patch} rows="3"/></div></DialogContent><DialogActions><Button onClick={onClose}>Cancel</Button><Button variant="contained" onClick={save}>Save</Button></DialogActions></Dialog>
}
const sampleEmployees=[{id:'e1',fullName:'Ravi Kumar',role:'Worker',salary:18000,phone:'+91-9111111111',address:'Farm quarters',tasks:'Milking, feeding, cleaning'}];
function TeamPage({setToast,currentUser}){
  const [users,setUsers]=useState([]); const [open,setOpen]=useState(false); const [editing,setEditing]=useState(null); const [confirmDelete,setConfirmDelete]=useState(null);
  const isAdmin=currentUser?.role===0;
  useEffect(()=>{load()},[]);
  async function load(){try{const r=await api.get('/auth/users');setUsers(r.data||[])}catch{setUsers([])}}
  async function remove(u){try{await api.delete(`/auth/users/${u.id}`);setToast('User removed');load()}catch(err){setToast(err?.response?.data?.message||'Could not remove user')}finally{setConfirmDelete(null)}}
  return <div className="grid">
    <div className="toolbar">
      <h2 style={{margin:0}}>Team</h2>
      {isAdmin && <Button startIcon={<Add/>} variant="contained" onClick={()=>{setEditing(null);setOpen(true)}}>Add User</Button>}
    </div>
    <div className="card table-wrap"><table className="table"><thead><tr><th>Name</th><th>Email</th><th>Role</th><th>Phone</th>{isAdmin && <th>Actions</th>}</tr></thead><tbody>
      {users.map(u=><tr key={u.id}>
        <td data-label="Name"><b>{u.fullName}</b></td>
        <td data-label="Email">{u.email}</td>
        <td data-label="Role">{roleNames[u.role]||'User'}</td>
        <td data-label="Phone">{u.phone||'-'}</td>
        {isAdmin && <td data-label="Actions"><IconButton size="small" onClick={()=>{setEditing(u);setOpen(true)}} title="Edit"><Edit fontSize="small"/></IconButton><IconButton size="small" onClick={()=>setConfirmDelete(u)} title="Remove" disabled={u.id===currentUser?.id}><Delete fontSize="small" color="error"/></IconButton></td>}
      </tr>)}
      {users.length===0 && <tr><td colSpan={isAdmin?5:4}><p className="muted">No team members found.</p></td></tr>}
    </tbody></table></div>
    {!isAdmin && <p className="muted">Only admins can manage team members.</p>}
    <AddUserDialog open={open} user={editing} onClose={()=>setOpen(false)} onSaved={()=>{setToast(editing?'User updated':'User created');setOpen(false);load()}}/>
    <Dialog open={!!confirmDelete} onClose={()=>setConfirmDelete(null)}><DialogTitle>Remove {confirmDelete?.fullName}?</DialogTitle><DialogContent><Typography className="muted">They'll lose access immediately. Their past expenses and history are kept.</Typography></DialogContent><DialogActions><Button onClick={()=>setConfirmDelete(null)}>Cancel</Button><Button color="error" variant="contained" onClick={()=>remove(confirmDelete)}>Remove</Button></DialogActions></Dialog>
  </div>
}

function AddUserDialog({open,user,onClose,onSaved}){
  const blank={fullName:'',email:'',password:'',role:2,phone:''};
  const [form,setForm]=useState(blank); const [error,setError]=useState('');
  useEffect(()=>{if(open){setForm(user?{fullName:user.fullName,email:user.email,password:'',role:user.role,phone:user.phone||''}:blank);setError('')}},[open,user]);
  function patch(e){setForm({...form,[e.target.name]:e.target.value})}
  async function save(){
    if(!form.fullName.trim()){setError('Full name is required.');return}
    if(!user && !form.email.trim()){setError('Email is required.');return}
    if(!user && (!form.password || form.password.length<6)){setError('Password must be at least 6 characters.');return}
    try{
      if(user) await api.put(`/auth/users/${user.id}`,{fullName:form.fullName,role:Number(form.role),phone:form.phone||null});
      else await api.post('/auth/users',{fullName:form.fullName,email:form.email,password:form.password,role:Number(form.role),phone:form.phone||null});
      onSaved();
    }
    catch(err){ setError(err?.response?.data?.message || 'Could not save user. Email may already be in use.'); }
  }
  return <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
    <DialogTitle>{user?'Edit User':'Add User'}</DialogTitle>
    <DialogContent>
      {error && <Alert severity="error" sx={{mb:2}}>{error}</Alert>}
      <div className="form-grid">
        <input className="input" name="fullName" placeholder="Full name" value={form.fullName} onChange={patch}/>
        <input className="input" name="email" type="email" placeholder="Email" value={form.email} onChange={patch} disabled={!!user}/>
        {!user && <input className="input" name="password" type="password" placeholder="Password (min 6 chars)" value={form.password} onChange={patch}/>}
        <select className="input" name="role" value={form.role} onChange={patch}>
          <option value={0}>Admin</option>
          <option value={1}>Manager</option>
          <option value={2}>Worker</option>
        </select>
        <input className="input" name="phone" placeholder="Phone (optional)" value={form.phone} onChange={patch}/>
      </div>
      {user && <Typography variant="body2" className="muted" sx={{mt:1}}>Email can't be changed here.</Typography>}
    </DialogContent>
    <DialogActions><Button onClick={onClose}>Cancel</Button><Button variant="contained" onClick={save}>{user?'Save Changes':'Create User'}</Button></DialogActions>
  </Dialog>
}
function ModulePage({name}){return <div className="card"><div className="toolbar"><h2 style={{margin:0,textTransform:'capitalize'}}>{name}</h2><Button variant="contained" startIcon={<Add/>}>New</Button></div><p className="muted">REST API and database model are ready for this module. Connect table/form fields to the matching endpoint as workflows mature.</p></div>}
function ReportsPage(){const reports=['animals','sales','expenses','income','inventory'];return <div className="grid">{reports.map(r=><div className="card" key={r}><h3 style={{textTransform:'capitalize'}}>{r} Report</h3><Button href={`${import.meta.env.VITE_API_URL||'http://localhost:5000/api'}/reports/${r}/csv`}>CSV</Button> <Button href={`${import.meta.env.VITE_API_URL||'http://localhost:5000/api'}/reports/${r}/excel`}>Excel</Button> <Button href={`${import.meta.env.VITE_API_URL||'http://localhost:5000/api'}/reports/${r}/pdf`}>PDF</Button></div>)}</div>}
function money(v){return new Intl.NumberFormat('en-IN',{style:'currency',currency:'INR',maximumFractionDigits:0}).format(v||0)} function label(k){return k.replace(/([A-Z])/g,' $1').replace(/^./,c=>c.toUpperCase())}
const sampleAnimals=[{tagNumber:'GVF-1001',name:'Lakshmi',species:'Cow',breed:'Jersey',status:'Active',currentValue:68000},{tagNumber:'GVF-2001',name:'Meera',species:'Goat',breed:'Boer',status:'Active',currentValue:12000}];
const sampleDashboard={totalAnimals:2,totalStock:148,monthlyIncome:2280,monthlyExpenses:4200,pendingPayments:5500,todaysSales:2280,recentActivities:[{action:'Seeded sample farm data',entityName:'System'}],incomeVsExpenses:[{label:'This Month',value:2280,secondaryValue:4200},{label:'Lifetime',value:2280,secondaryValue:4200}],animalCountByCategory:[{label:'Cow',value:1},{label:'Goat',value:1}]};
