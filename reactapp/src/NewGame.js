import React, { Component } from 'react';
import Input from '@mui/joy/Input';
import Button from '@mui/joy/Button';
import { IconButton, List, ListItem, FormControl } from '../node_modules/@mui/joy/index';
import { Close } from '../node_modules/@mui/icons-material/index';

class NewGame extends Component {
    constructor(props) {
        super(props);
        this.state = {players : [], playerFieldValue: "", ready: false, error: "", gameId: ""}
    }

    componentDidMount() {
        this.setState({ready: true});   
    }

    removePlayer(player) {
        let newState = this.state.players.filter(i => i !== player);
        this.setState({players : newState});
    }

    addPlayer(event)  {
        event.preventDefault();
        let player = this.state.playerFieldValue;
        let newState = this.state.players;
        newState.push(player);
        this.setState({ players: newState, playerFieldValue: "" });
    }

    updateInputValue(evt) {
        this.setState({ playerFieldValue: evt.target.value });
    }

    resetInput() {
        this.setState({ playerFieldValue: ""});

    }

    updateGameId(evt) {
        this.setState({ gameId: evt.target.value });
    }

    createGame() {
        const len = this.state.players.length;
        if (len !== 5 && len !== 7 && len !== 8 && len !== 10) {
            this.setState({ error: "invalid # of players" })
            console.log("Invalid # of players")
        }
        else {
            this.props.createGame(this.state.players)
        }
    }

    getGame(evt) {
        evt.preventDefault();
        if (this.state.gameId.length !== 4) {
            this.setState({error: "Game ID must be 4 characters"})
        }
        this.props.getGame(this.state.gameId);
    }
    

    render() {
        let players = this.state.players.map(player => <ListItem> {player} <IconButton color='danger' sx={{ marginLeft: "10px" }} onClick={() => this.removePlayer(player)}>
            <Close />
        </IconButton></ListItem>);

        return (
            <div>
                {this.state.ready ? <div>
                    <FormControl onSubmit={(event) => this.addPlayer(event)}> <Input size="md" sx={{ width: "300px", marginLeft: "10px" }} value={this.state.playerFieldValue} placeholder="Enter player's names one at a time" onChange={(evt) => this.updateInputValue(evt)}></Input> </FormControl>

                    <List>{players}</List>
                    <Button onClick={() => this.createGame()} sx={{ margin: "10px", width: "100px" }}> Create Game </Button>

                    <FormControl onSubmit={(event) => this.getGame(event)}>
                        <Input size="md" sx={{ width: "300px", margin: "10px" }} placeholder="Enter a game ID"  value={this.state.gameId} onChange={(evt) => this.updateGameId(evt)}></Input>
                        <Button type='submit' sx={{ margin: "10px", width: "100px" }} size="sm"> Join Game </Button>
                    </FormControl>

                    {this.state.error}
                </div> 
                 : <div> Loading... </div>}
            </div>
        );
    }

}
export default NewGame;
